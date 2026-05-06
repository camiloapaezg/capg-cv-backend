using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Validators.Internal;
using capg_hv_backend.Domain.Entities;
using Carter;
using Carter.ModelBinding;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace capg_hv_backend.Endpoints.Internal;

public sealed class GeneralDetailsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("api/v1/users/{userId:guid}/general")
            .WithTags("General Details");

        group.MapGet("/", GetAll)
            .Produces<List<GeneralDetails>>()
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("{id:guid}", Get)
            .Produces<GeneralDetails>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", Create)
            .Produces<GeneralDetails>()
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
        GeneralDetailsAddValidator validator,
        [FromBody] GeneralDetails entity,
        [FromServices] IRepository<User> usersRepo,
        [FromServices] IRepository<GeneralDetails> generalDetailsRepo)
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

        GeneralDetails newEntity = (GeneralDetails)entity.Clone();
        newEntity.UserId = userId;

        GeneralDetails? result = await generalDetailsRepo.Create(newEntity);
        if (result is null)
        {
            return Results.Problem("Error creating entity.", statusCode: 500);
        }

        return Results.Created($"api/v1/users/{userId}/general/{result.Id}", result);
    }

    private async Task<IResult> Delete(
        Guid userId,
        Guid id,
        [FromServices] IRepository<User> usersRepo,
        [FromServices] IRepository<GeneralDetails> generalDetailsRepo)
    {
        bool exists = await usersRepo.Exists(userId);
        if (!exists)
        {
            return Results.BadRequest($"The user with Id '{userId}' does not exist in database");
        }

        GeneralDetails? result = await generalDetailsRepo.Delete(id);
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
        [FromServices] IRepository<GeneralDetails> generalDetailsRepo)
    {
        bool exists = await usersRepo.Exists(userId);
        if (!exists)
        {
            return Results.BadRequest($"The user with Id '{userId}' does not exist in database");
        }

        GeneralDetails? result = await generalDetailsRepo.Get(id);
        if (result is null)
        {
            return Results.BadRequest("The entity does not exist in the database.");
        }

        return Results.Ok(result);
    }

    private async Task<IResult> GetAll(
        Guid userId,
        [FromServices] IRepository<GeneralDetails> repository)
    {
        List<GeneralDetails> list = await repository.List(userId);
        return Results.Ok(list);
    }

    private async Task<IResult> Update(
        Guid userId,
        Guid id,
        GeneralDetailsUpdateValidator validator,
        [FromBody] GeneralDetails entity,
        [FromServices] IRepository<GeneralDetails> repository)
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

        GeneralDetails newEntity = (GeneralDetails)entity.Clone();
        newEntity.UserId = userId;

        GeneralDetails? result = await repository.Update(newEntity);
        if (result is null)
        {
            return Results.BadRequest($"Error updating the entity with the Id '{id}'.");
        }

        return Results.NoContent();
    }
}