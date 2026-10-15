using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Validators.Internal;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Endpoints.Entities;
using Carter;
using Carter.ModelBinding;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace capg_hv_backend.Endpoints.Internal;

public sealed class PublicationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("api/v1/users/{userId:guid}/publication")
            .WithTags("Publications");

        group.MapGet("/", ReadAll)
            .Produces<List<Publication>>()
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("{id:guid}", Read)
            .Produces<Publication>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", Create)
            .Produces<Publication>()
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
    }

    private async Task<IResult> Create(
        Guid userId,
        PublicationValidator validator,
        [FromBody] PublicationDto request,
        [FromServices] IListableRepository<User> userRepository,
        [FromServices] IListableRepository<Publication> publicationRepository)
    {
        ValidationResult validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Results.UnprocessableEntity(validationResult.GetFormattedErrors());
        }

        User? user = await userRepository.ReadAsync(userId);
        if (user is null)
        {
            return Results.BadRequest($"The user with Id '{userId}' does not exist in database");
        }

        Publication? newEntity = new()
        {
            Title = request.Title,
            Type = request.Type,
            Location = request.Location,
            PublishedAt = request.PublishedAt,
            Doi = request.Doi,
            UserId = user.Id,
        };

        newEntity = await publicationRepository.CreateAsync(newEntity);
        if (newEntity is null)
        {
            return Results.InternalServerError($"Error creating publication entry for the user with Id '{userId}'.");
        }

        return Results.Created($"api/v1/users/{userId}/publication/{newEntity.Id}", newEntity);
    }

    private async Task<IResult> Delete(
        Guid userId,
        Guid id,
        [FromServices] IListableRepository<Publication> repository)
    {
        Publication? entry = await repository.ReadAsync(id);
        if (entry is null)
        {
            return Results.BadRequest($"The publication entry with Id '{id}', belonging to the user with Id '{userId}' was not found.");
        }

        if (entry.UserId != userId)
        {
            return Results.BadRequest("The publication entry user Id does not correspond to the one read from the request.");
        }

        entry = await repository.DeleteAsync(id);
        if (entry is null)
        {
            return Results.InternalServerError($"Error deleting publication entry with Id '{id}' related to the user with id '{userId}'");
        }

        return Results.NoContent();
    }

    private async Task<IResult> Read(
        Guid userId,
        Guid id,
        [FromServices] IListableRepository<Publication> repository)
    {
        Publication? existing = await repository.ReadAsync(id);
        if (existing is null)
        {
            return Results.BadRequest("The entity does not exist in the database.");
        }

        if (existing.UserId != userId)
        {
            return Results.BadRequest("The entry user Id does not correspond to the one read from the request.");
        }

        return Results.Ok(existing);
    }

    private async Task<IResult> ReadAll(
        Guid userId,
        [FromServices] IListableRepository<Publication> repository)
    {
        List<Publication> list = await repository.List(userId);
        return Results.Ok(list);
    }

    private async Task<IResult> Update(
        Guid userId,
        Guid id,
        PublicationValidator validator,
        [FromBody] PublicationDto request,
        [FromServices] IListableRepository<Publication> repository)
    {
        // Validates request.
        ValidationResult validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Results.UnprocessableEntity(validationResult.GetFormattedErrors());
        }

        Publication? existing = await repository.ReadAsync(id);
        if (existing is null)
        {
            return Results.BadRequest("The entity does not exist in the database.");
        }

        if (existing.UserId != userId)
        {
            return Results.BadRequest("The entry user Id does not correspond to the one read from the request.");
        }

        Publication? newEntity = (Publication)existing.Clone();
        newEntity.Title = request.Title;
        newEntity.Type = request.Type;
        newEntity.Location = request.Location;
        newEntity.PublishedAt = request.PublishedAt;
        newEntity.Doi = request.Doi;

        newEntity = await repository.UpdateAsync(newEntity);
        if (newEntity is null)
        {
            return Results.InternalServerError($"Error updating the publication entry with the Id '{id}'.");
        }

        return Results.NoContent();
    }
}