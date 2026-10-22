using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Validators.Internal;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Endpoints.Abstractions;
using capg_hv_backend.Endpoints.Entities;
using Carter;
using Carter.ModelBinding;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace capg_hv_backend.Endpoints.Internal;

public sealed class GeneralDetailsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("api/v1/users/{userId:guid}/general")
            .WithTags("General Details");

        group.MapGet("/", Read)
            .Produces<GeneralDetails>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/", Update)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);

        // Photo
        group.MapGet("/photo", DownloadPhoto)
            .WithDescription("Downloads the user's profile photo.")
            .DisableAntiforgery()
            .Produces<FileStreamHttpResult>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/photo", UploadPhoto)
            .WithDescription("Uploads the user's profile photo.")
            .DisableAntiforgery()
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private async Task<IResult> DownloadPhoto(
        Guid userId,
        [FromServices] IFilesStorageOperations _fileStorageOps,
        [FromServices] ICrudRepository<GeneralDetails> repository)
    {
        // Validates user id
        GeneralDetails? details = await repository.ReadAsync(userId);
        if (details is null)
        {
            return Results.BadRequest("The general details entry does not exist in the database.");
        }

        if (details.PhotoFileId is null)
        {
            return Results.BadRequest("The user does not have a profile photo registered in database.");
        }

        // Downloads the file.
        FileHttpOperationResultDto<FileDownloadResultDto> result = await _fileStorageOps.DownloadFileAsync(details.PhotoFileId.Value);
        if (result.Status != HttpStatusCode.OK || result.Value is null)
        {
            return Results.Problem(statusCode: (int)result.Status, detail: result.Message);
        }

        return TypedResults.File(fileStream: new MemoryStream(result.Value.Data), contentType: "application/octet-stream", fileDownloadName: result.Value.FileName);
    }

    private async Task<IResult> Read(
        Guid userId,
        [FromServices] ICrudRepository<GeneralDetails> repository)
    {
        GeneralDetails? result = await repository.ReadAsync(userId);
        if (result is null)
        {
            return Results.BadRequest("The general details entry does not exist in the database.");
        }

        return Results.Ok(result);
    }

    private async Task<IResult> Update(
        Guid userId,
        GeneralDetailsValidator validator,
        [FromBody] GeneralDetailsDto request,
        [FromServices] ICrudRepository<GeneralDetails> repository)
    {
        ValidationResult validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Results.UnprocessableEntity(validationResult.GetFormattedErrors());
        }

        GeneralDetails? existing = await repository.ReadAsync(userId);
        if (existing is null)
        {
            return Results.BadRequest("The general details entry does not exist in the database.");
        }

        // Updates the entity
        GeneralDetails? newEntry = (GeneralDetails)existing.Clone();
        newEntry.Title = request.Title;
        newEntry.Description = request.Description;
        newEntry.Languages = request.Languages;
        newEntry.TechnicalSkills = request.TechnicalSkills;

        newEntry = await repository.UpdateAsync(newEntry);
        if (newEntry is null)
        {
            return Results.InternalServerError($"Error updating the general details of the user with Id '{userId}'.");
        }

        return Results.NoContent();
    }

    private async Task<IResult> UploadPhoto(
        Guid userId,
        IFormFile file,
        [FromServices] IFilesStorageOperations _fileStorageOps,
        [FromServices] ICrudRepository<GeneralDetails> repository)
    {
        if (file is null)
        {
            return Results.BadRequest("The file content is null.");
        }

        // Validates user id
        GeneralDetails? existing = await repository.ReadAsync(userId);
        if (existing is null)
        {
            return Results.BadRequest("The general details entry does not exist in the database.");
        }

        // Starts file upload.
        FileHttpOperationResultDto<FileScanRequestDto> result = await _fileStorageOps.StartUploadFileAsync(userId, file);
        if (result.Status != HttpStatusCode.Accepted || result.Value is null)
        {
            return Results.Problem(statusCode: (int)result.Status, detail: result.Message);
        }

        // Updates the Photo File Id in the general details property
        existing.PhotoFileId = result.Value.Id;
        existing = await repository.UpdateAsync(existing);
        if (existing is null)
        {
            return Results.InternalServerError($"Error updating the general details photo file Id of the user with Id '{userId}'.");
        }

        return Results.Accepted();
    }
}