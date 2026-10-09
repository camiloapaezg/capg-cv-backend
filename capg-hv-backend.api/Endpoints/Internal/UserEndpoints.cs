using capg_hv_backend.Application.Channels.Abstractions;
using capg_hv_backend.Application.Channels.Entities;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Repositories.Entities;
using capg_hv_backend.Application.Validators.Internal;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Endpoints.Entities;
using Carter;
using Carter.ModelBinding;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace capg_hv_backend.Endpoints.Internal;

public sealed class UserEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("api/v1/users")
            .WithTags("Users");

        group.MapGet("/", GetAll)
            .Produces<List<User>>()
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("{id:guid}", Get)
            .Produces<User>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", Create)
            .Produces<User>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("{id:guid}", Delete)
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("{id:guid}", Update)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);

        // Files
        group.MapGet("{id:guid}/photo", DownloadPhoto)
            .WithDescription("Downloads the user's profile photo")
            .Produces<FileStreamHttpResult>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private async Task<IResult> Create(
        UserAddValidator validator,
        [FromBody] UserCreateRequestDto request,
        [FromServices] IRepository<User> usersRepository,
        [FromServices] IRepository<GeneralDetails> generalDetailsRepository,
        [FromServices] IRepository<PersonalDetails> personalDetailsRepository)
    {
        // Validates request
        ValidationResult validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Results.UnprocessableEntity(validationResult.GetFormattedErrors());
        }

        // Creates the new user
        User? newUser = new()
        {
          FirstName = request.FirstName,
          LastName = request.LastName,
          EmailAddress = request.EmailAddress  
        };

        newUser = await usersRepository.Create(newUser);
        if (newUser is null)
        {
            return Results.Problem("Error creating entity.", statusCode: 500);
        }

        // Creates General and Project Details
        var generalDetails = new GeneralDetails()
        {
            Title = $"{newUser.FirstName} {newUser.LastName}",
            UserId = newUser.Id
        };

        generalDetails = await generalDetailsRepository.Create(generalDetails);
        if(generalDetails is null)
        {
            
        }

        return Results.Created($"api/v1/users/{newUser.Id}", newUser);
    }

    private async Task<IResult> Delete(Guid id,
        [FromServices] IRepository<User> usersRepository,
        [FromServices] IRepository<FileMetaData> metadataRepository,
        [FromServices] IChannel<FileDeleteRequestDto> channel)
    {
        User? result = await usersRepository.Delete(id);
        if (result is null)
        {
            return Results.BadRequest($"Error deleting the entity with Id '{id}'");
        }

        // Sends request for files removal
        List<FileMetaData> files = await metadataRepository.List(id);
        if (files.Count > 0)
        {
            files.ForEach(async file => await channel.WriteAsync(new FileDeleteRequestDto(file.Id, file.Name)));
        }

        return Results.Accepted();
    }

    private async Task<IResult> Get(Guid id, [FromServices] IRepository<User> repository)
    {
        User? result = await repository.Get(id);
        if (result is null)
        {
            return Results.BadRequest("The entity does not exist in the database.");
        }

        return Results.Ok(result);
    }

    private async Task<IResult> GetAll([FromServices] IRepository<User> repository)
    {
        List<User> list = await repository.List();
        return Results.Ok(list);
    }

    private async Task<IResult> Update(
        Guid id,
        UserUpdateValidator validator,
        [FromBody] User entity,
        [FromServices] IRepository<User> repository)
    {
        if (id != entity.Id)
        {
            return Results.BadRequest("The route Id is not the same as the entity Id");
        }

        ValidationResult validationResult = validator.Validate(entity);
        if (!validationResult.IsValid)
        {
            return Results.UnprocessableEntity(validationResult.GetFormattedErrors());
        }

        User? result = await repository.Update(entity);
        if (result is null)
        {
            return Results.BadRequest($"Error updating the entity with the Id '{id}'.");
        }

        return Results.NoContent();
    }

    private async Task<IResult> DownloadPhoto(
        Guid id,
        [FromServices] IRepository<GeneralDetails> generalDetailsRepository,
        [FromServices] IRepository<FileMetaData> metadataRepository,
        [FromServices] IFilesRepository filesRepository)
    {
        // Validates user id
        GeneralDetails? details = await generalDetailsRepository.Get(id);
        if (details is null)
        {
            return Results.BadRequest("The user does not exist in the database.");
        }

        if(details.PhotoFileId is null)
        {
            return Results.BadRequest("The user does not have a profile photo.");
        }

        // Gets the metadata registry
        FileMetaData? metadata = await metadataRepository.Get(details.PhotoFileId.Value);
        if (metadata is null)
        {
            return Results.BadRequest("The photo is not registered in the database.");
        }

        // Gets the file
        FileOperationResult<byte[]> downloaded = await filesRepository.Download(metadata.Id);
        if (downloaded.StatusCode != HttpStatusCode.OK || downloaded.Data is null)
        {
            return Results.InternalServerError(downloaded.Message);
        }

        return TypedResults.File(fileStream: new MemoryStream(downloaded.Data), contentType: "application/octet-stream", fileDownloadName: metadata.Name);
    }
}