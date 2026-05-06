using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Validators.Internal;
using capg_hv_backend.Domain.Entities;
using Carter;
using Carter.ModelBinding;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace capg_hv_backend.Endpoints.Internal;

public sealed class PersonalDetailsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("api/v1/users/{userId:guid}/personal")
            .WithTags("Personal Details");

        group.MapGet("/", GetAll)
            .Produces<List<PersonalDetails>>()
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("{id:guid}", Get)
            .Produces<PersonalDetails>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", Create)
            .Produces<PersonalDetails>()
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
        PersonalDetailsAddValidator validator,
        [FromBody] PersonalDetails entity,
        [FromServices] IRepository<User> usersRepo,
        [FromServices] IRepository<PersonalDetails> personalDetailsRepo)
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

        PersonalDetails newEntity = (PersonalDetails)entity.Clone();
        newEntity.UserId = userId;

        PersonalDetails? result = await personalDetailsRepo.Create(newEntity);
        if (result is null)
        {
            return Results.Problem("Error creating entity.", statusCode: 500);
        }

        return Results.Created($"api/v1/users/{userId}/personal/{result.Id}", result);
    }

    private async Task<IResult> Delete(
        Guid userId,
        Guid id,
        [FromServices] IRepository<User> usersRepo,
        [FromServices] IRepository<PersonalDetails> personalDetailsRepo)
    {
        bool exists = await usersRepo.Exists(userId);
        if (!exists)
        {
            return Results.BadRequest($"The user with Id '{userId}' does not exist in database");
        }

        PersonalDetails? result = await personalDetailsRepo.Delete(id);
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
        [FromServices] IRepository<PersonalDetails> personalDetailsRepo)
    {
        bool exists = await usersRepo.Exists(userId);
        if (!exists)
        {
            return Results.BadRequest($"The user with Id '{userId}' does not exist in database");
        }

        PersonalDetails? result = await personalDetailsRepo.Get(id);
        if (result is null)
        {
            return Results.BadRequest("The entity does not exist in the database.");
        }

        return Results.Ok(result);
    }

    private async Task<IResult> GetAll(
        Guid userId,
        [FromServices] IRepository<PersonalDetails> repository)
    {
        List<PersonalDetails> list = await repository.List(userId);
        return Results.Ok(list);
    }

    private async Task<IResult> Update(
        Guid userId,
        Guid id,
        PersonalDetailsUpdateValidator validator,
        [FromBody] PersonalDetails entity,
        [FromServices] IRepository<PersonalDetails> repository)
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

        PersonalDetails newEntity = (PersonalDetails)entity.Clone();
        newEntity.UserId = userId;

        PersonalDetails? result = await repository.Update(newEntity);
        if (result is null)
        {
            return Results.BadRequest($"Error updating the entity with the Id '{id}'.");
        }

        return Results.NoContent();
    }
}