using capg_hv_backend.Application.FilesValidator.Abstractions;
using capg_hv_backend.Application.FilesValidator.Entities;
using capg_hv_backend.Application.FilesValidator.Internal;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Repositories.Entities;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Endpoints.Entities;
using capg_hv_backend.Infrastructure.MessageBroker;
using capg_hv_backend.Infrastructure.MessageBroker.Abstractions;
using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace capg_hv_backend.Endpoints.Internal;

public sealed class FilesEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("api/v1/users/{userId:guid}/files")
            .WithTags("Files")
            .DisableAntiforgery();

        group.MapGet("{id:guid}", Download)
            .Produces<FileStreamHttpResult>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", Upload)
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("{id:guid}", Delete)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private async Task<IResult> Delete(
        Guid userId,
        Guid id,
        [FromServices] IRepository<User> usersRepository,
        [FromServices] IRepository<FileMetaData> metadataRepository,
        [FromServices] IFilesRepository filesRepository)
    {
        // Validates user id
        bool existing = await usersRepository.Exists(userId);
        if (!existing)
        {
            return Results.BadRequest("The user does not exist in the database.");
        }

        // Gets the metadata
        FileMetaData? metadata = await metadataRepository.Get(id);
        if (metadata is null)
        {
            return Results.BadRequest("The file is not registered in the database.");
        }

        // Deletes the file and metadata
        FileOperationResult<object> deleted = await filesRepository.Delete(metadata.Id);
        if (deleted.StatusCode != HttpStatusCode.NoContent)
        {
            return Results.InternalServerError(deleted.Message);
        }

        metadata = await metadataRepository.Delete(id);
        if (metadata is null)
        {
            return Results.BadRequest("Error deleting file metadata.");
        }

        return Results.NoContent();
    }

    private async Task<IResult> Download(
        Guid userId,
        Guid id,
        [FromServices] IRepository<User> usersRepository,
        [FromServices] IRepository<FileMetaData> metadataRepository,
        [FromServices] IFilesRepository filesRepository)
    {
        // Validates user id
        bool existing = await usersRepository.Exists(userId);
        if (!existing)
        {
            return Results.BadRequest("The user does not exist in the database.");
        }

        // Gets the metadata registry
        FileMetaData? metadata = await metadataRepository.Get(id);
        if (metadata is null)
        {
            return Results.BadRequest("The file is not registered in the database.");
        }

        // Gets the file
        FileOperationResult<byte[]> downloaded = await filesRepository.Download(id);
        if (downloaded.StatusCode != HttpStatusCode.OK || downloaded.Data is null)
        {
            return Results.InternalServerError(downloaded.Message);
        }

        return TypedResults.File(fileStream: new MemoryStream(downloaded.Data), contentType: "application/octet-stream", fileDownloadName: metadata.Name);
    }

    private async Task<IResult> Upload(
        Guid userId,
        IFormFile file,
        [FromQuery] string? fileName,
        [FromServices] IOptions<MessageBrokerOptions> options,
        [FromServices] IFilesValidator fileValidator,
        [FromServices] IRepository<User> usersRepository,
        [FromServices] IFilesRepository filesRepository,
        [FromServices] IMessageBroker messageBroker)
    {
        if (file is null)
        {
            return Results.BadRequest("The file content is null.");
        }

        // Validates user id
        bool existing = await usersRepository.Exists(userId);
        if (!existing)
        {
            return Results.BadRequest("The user does not exist in the database.");
        }

        // Prepares file name
        string _fileName = file.FileName;
        if (fileName is not null)
        {
            string extension = Path.GetExtension(file.FileName);
            _fileName = fileName + extension;
        }

        // Prepares file content.
        using MemoryStream stream = new();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        // Validates file content
        ValidationResult validationResult = await fileValidator.ValidateFileAsync(stream.ToArray(), _fileName);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Message);
        }

        // Creates the request.
        _fileName = FilesValidator.GetSanitizedFileName(_fileName);
        FileScanRequestDto scanRequest = new(Guid.NewGuid(), userId, _fileName);

        // Saves file to quarantine blob.
        FileOperationResult<string> uploaded = await filesRepository.UploadToQuarantine(scanRequest.Id, stream);
        if (uploaded.StatusCode != HttpStatusCode.OK)
        {
            return Results.InternalServerError(uploaded.Message);
        }

        // Sends request to message broker queue.
        string queueName = options.Value.Queues.FilesScanner;
        byte[] body = JsonSerializer.SerializeToUtf8Bytes(scanRequest);
        await messageBroker.SendMessage(queueName, body);

        return Results.Accepted();
    }
}