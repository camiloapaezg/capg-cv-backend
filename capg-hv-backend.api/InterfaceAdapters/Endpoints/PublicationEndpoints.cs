using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Validators.Internal;
using capg_hv_backend.Domain.Entities;
using Carter;
using Carter.ModelBinding;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace capg_hv_backend.InterfaceAdapters.Endpoints;

public sealed class PublicationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("api/v1/users/{userId:guid}/publication")
            .WithTags("Publications");

        group.MapGet("/", GetAll)
            .Produces<List<Publication>>()
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("{id:guid}", Get)
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
        PublicationAddValidator validator,
        [FromBody] Publication entity,
        [FromServices] IRepository<User> usersRepo,
        [FromServices] IRepository<Publication> publicationRepo)
    {
        ValidationResult validationResult = validator.Validate(entity);
        if (!validationResult.IsValid)
        {
            return Results.UnprocessableEntity(validationResult.GetFormattedErrors());
        }

        bool exists = await usersRepo.Exists(userId);
        if (!exists)
        {
            return Results.BadRequest($"The user with Id '{userId}' does not exist in database");
        }

        Publication newEntity = (Publication)entity.Clone();
        newEntity.UserId = userId;

        Publication? result = await publicationRepo.Create(newEntity);
        if (result is null)
        {
            return Results.Problem("Error creating entity.", statusCode: 500);
        }

        return Results.Created($"api/v1/users/{userId}/publication/{result.Id}", result);
    }

    private async Task<IResult> Delete(
        Guid userId,
        Guid id,
        [FromServices] IRepository<User> usersRepo,
        [FromServices] IRepository<Publication> publicationRepo)
    {
        bool exists = await usersRepo.Exists(userId);
        if (!exists)
        {
            return Results.BadRequest($"The user with Id '{userId}' does not exist in database");
        }

        Publication? result = await publicationRepo.Delete(id);
        if (result is null)
        {
            return Results.BadRequest($"Error deleting entity with Id '{id}'");
        }

        return Results.NoContent();
    }

    private async Task<IResult> Get(
        Guid userId,
        Guid id,
        [FromServices] IRepository<User> usersRepo,
        [FromServices] IRepository<Publication> publicationRepo)
    {
        bool exists = await usersRepo.Exists(userId);
        if (!exists)
        {
            return Results.BadRequest($"The user with Id '{userId}' does not exist in database");
        }

        Publication? result = await publicationRepo.Get(id);
        if (result is null)
        {
            return Results.BadRequest("The entity does not exist in the database.");
        }

        return Results.Ok(result);
    }

    private async Task<IResult> GetAll(
        Guid userId,
        [FromServices] IRepository<Publication> repository)
    {
        List<Publication> list = await repository.List(userId);
        return Results.Ok(list);
    }

    private async Task<IResult> Update(
        Guid userId,
        Guid id,
        PublicationUpdateValidator validator,
        [FromBody] Publication entity,
        [FromServices] IRepository<Publication> repository)
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

        Publication newEntity = (Publication)entity.Clone();
        newEntity.UserId = userId;

        Publication? result = await repository.Update(newEntity);
        if (result is null)
        {
            return Results.BadRequest($"Error updating the entity with the Id '{id}'.");
        }

        return Results.NoContent();
    }
}