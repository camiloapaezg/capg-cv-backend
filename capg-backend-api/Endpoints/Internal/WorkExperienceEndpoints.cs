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

public sealed class WorkExperienceEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("api/v1/users/{userId:guid}/experience")
            .WithTags("Work Experience");

        group.MapGet("/", ReadAll)
            .Produces<List<WorkExperience>>()
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("{id:guid}", Read)
            .Produces<WorkExperience>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", Create)
            .Produces<WorkExperience>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("{id:guid}", Delete)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("{id:guid}", Update)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);

        // Files
        group.MapGet("{id:guid}/file", DownloadFile)
            .WithDescription("Downloads the file.")
            .DisableAntiforgery()
            .Produces<FileStreamHttpResult>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("{id:guid}/file", UploadFile)
            .WithDescription("Uploads a file.")
            .DisableAntiforgery()
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private async Task<IResult> Create(
        Guid userId,
        WorkExperienceValidator validator,
        [FromBody] WorkExperienceDto request,
        [FromServices] IListableRepository<User> usersRepository,
        [FromServices] IListableRepository<WorkExperience> experienceRepository)
    {
        ValidationResult validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Results.UnprocessableEntity(validationResult.GetFormattedErrors());
        }

        User? user = await usersRepository.ReadAsync(userId);
        if (user is null)
        {
            return Results.BadRequest($"The user with Id '{userId}' does not exist in database.");
        }

        WorkExperience? newEntity = new()
        {
            JobTitle = request.JobTitle,
            Company = request.Company,
            Location = request.Location,
            Description = request.Description,
            From = request.From,
            Until = request.Until,
            ContactName = request.ContactName,
            ContactNumber = request.ContactNumber,
            UserId = user.Id
        };

        newEntity = await experienceRepository.CreateAsync(newEntity);
        if (newEntity is null)
        {
            return Results.InternalServerError($"Error creating work experience entry for the user with Id '{userId}'.");
        }

        return Results.Created($"api/v1/users/{userId}/experience/{newEntity.Id}", newEntity);
    }

    private async Task<IResult> Delete(
        Guid userId,
        Guid id,
        [FromServices] IListableRepository<WorkExperience> repository)
    {
        WorkExperience? entry = await repository.ReadAsync(id);
        if (entry is null)
        {
            return Results.BadRequest($"The work experience entry with Id '{id}' related to the user with Id '{userId}' was not found.");
        }

        if (entry.UserId != userId)
        {
            return Results.BadRequest("The work experience entry user Id does not correspond to the one read from the request.");
        }

        WorkExperience? result = await repository.DeleteAsync(id);
        if (result is null)
        {
            return Results.InternalServerError($"Error deleting work experience entry with Id '{id}' related to the user with id '{userId}'.");
        }

        return Results.NoContent();
    }

    private async Task<IResult> DownloadFile(
        Guid userId,
        Guid id,
        [FromServices] IFilesStorageOperations _fileStorageOps,
        [FromServices] IListableRepository<WorkExperience> repository)
    {
        // Validates entity
        WorkExperience? entry = await repository.ReadAsync(id);
        if (entry is null)
        {
            return Results.BadRequest($"The work experience entry with Id '{id}', belonging to the user with Id '{userId}' was not found.");
        }

        if (entry.UserId != userId)
        {
            return Results.BadRequest("The work experience entry user Id does not correspond to the one read from the request.");
        }

        if (entry.FileId is null)
        {
            return Results.BadRequest($"There is no file attached to the work experience entry with Id '{id}'.");
        }

        // Downloads the file.
        FileHttpOperationResultDto<FileDownloadResultDto> result = await _fileStorageOps.DownloadFileAsync(entry.FileId.Value);
        if (result.Status != HttpStatusCode.OK || result.Value is null)
        {
            return Results.Problem(statusCode: (int)result.Status, detail: result.Message);
        }

        return TypedResults.File(fileStream: new MemoryStream(result.Value.Data), contentType: "application/octet-stream", fileDownloadName: result.Value.FileName);
    }

    private async Task<IResult> Read(
        Guid userId,
        Guid id,
        [FromServices] IListableRepository<WorkExperience> repository)
    {
        WorkExperience? existing = await repository.ReadAsync(id);
        if (existing is null)
        {
            return Results.BadRequest($"The work experience entry with Id '{id}', belonging to the user with Id '{userId}' was not found.");
        }

        if (existing.UserId != userId)
        {
            return Results.BadRequest("The work experience entry user Id does not correspond to the one read from the request.");
        }

        return Results.Ok(existing);
    }

    private async Task<IResult> ReadAll(
        Guid userId,
        [FromServices] IListableRepository<WorkExperience> repository)
    {
        List<WorkExperience> list = await repository.List(userId);
        return Results.Ok(list);
    }

    private async Task<IResult> Update(
        Guid userId,
        Guid id,
        WorkExperienceValidator validator,
        [FromBody] WorkExperienceDto request,
        [FromServices] IListableRepository<WorkExperience> repository)
    {
        ValidationResult validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Results.UnprocessableEntity(validationResult.GetFormattedErrors());
        }

        WorkExperience? existing = await repository.ReadAsync(id);
        if (existing is null)
        {
            return Results.BadRequest($"The work experience entry with Id '{id}', belonging to the user with Id '{userId}' was not found.");
        }

        if (existing.UserId != userId)
        {
            return Results.BadRequest("The work experience entry user Id does not correspond to the one read from the request.");
        }

        WorkExperience? newEntity = (WorkExperience)existing.Clone();
        newEntity.JobTitle = request.JobTitle;
        newEntity.Company = request.Company;
        newEntity.Location = request.Location;
        newEntity.Description = request.Description;
        newEntity.From = request.From;
        newEntity.Until = request.Until;
        newEntity.ContactName = request.ContactName;
        newEntity.ContactNumber = request.ContactNumber;

        newEntity = await repository.UpdateAsync(newEntity);
        if (newEntity is null)
        {
            return Results.InternalServerError($"Error updating the work experience entry with the Id '{id}' related to the user with Id '{userId}'.");
        }

        return Results.NoContent();
    }

    private async Task<IResult> UploadFile(
        Guid userId,
        Guid id,
        IFormFile file,
        [FromServices] IFilesStorageOperations _fileStorageOps,
        [FromServices] IListableRepository<WorkExperience> repository)
    {
        if (file is null)
        {
            return Results.BadRequest("There is no file attached to the request.");
        }

        // Validates id
        WorkExperience? existing = await repository.ReadAsync(id);
        if (existing is null)
        {
            return Results.BadRequest($"The work experience entry with Id '{id}' related to the user with Id '{userId}' was not found.");
        }

        if (existing.UserId != userId)
        {
            return Results.BadRequest("The work experience entry user Id does not correspond to the one read from the request.");
        }

        // Starts file upload.
        FileHttpOperationResultDto<FileScanRequestDto> result = await _fileStorageOps.StartUploadFileAsync(userId, file);
        if (result.Status != HttpStatusCode.Accepted || result.Value is null)
        {
            return Results.Problem(statusCode: (int)result.Status, detail: result.Message);
        }

        // Updates the file Id in the entry.
        existing.FileId = result.Value.Id;
        existing = await repository.UpdateAsync(existing);
        if (existing is null)
        {
            return Results.InternalServerError($"Error updating the FileId of the work experience entry with Id '{id}'.");
        }

        return Results.Accepted();
    }
}