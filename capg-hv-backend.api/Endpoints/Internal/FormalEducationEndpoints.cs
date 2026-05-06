using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Validators.Internal;
using capg_hv_backend.Domain.Entities;
using Carter;
using Carter.ModelBinding;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace capg_hv_backend.Endpoints.Internal;

public sealed class FormalEducationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("api/v1/users/{userId:guid}/education")
            .WithTags("Formal Education");

        group.MapGet("/", GetAll)
            .Produces<List<FormalEducation>>()
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("{id:guid}", Get)
            .Produces<FormalEducation>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", Create)
            .Produces<FormalEducation>()
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
        FormalEducationAddValidator validator,
        [FromBody] FormalEducation entity,
        [FromServices] IRepository<User> usersRepo,
        [FromServices] IRepository<FormalEducation> educationRepo)
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

        FormalEducation newEntity = (FormalEducation)entity.Clone();
        newEntity.UserId = userId;

        FormalEducation? result = await educationRepo.Create(newEntity);
        if (result is null)
        {
            return Results.Problem("Error creating entity.", statusCode: 500);
        }

        return Results.Created($"api/v1/users/{userId}/education/{result.Id}", result);
    }

    private async Task<IResult> Delete(
        Guid userId,
        Guid id,
        [FromServices] IRepository<User> usersRepo,
        [FromServices] IRepository<FormalEducation> educationRepo)
    {
        bool exists = await usersRepo.Exists(userId);
        if (!exists)
        {
            return Results.BadRequest($"The user with Id '{userId}' does not exist in database");
        }

        FormalEducation? result = await educationRepo.Delete(id);
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
        [FromServices] IRepository<FormalEducation> educationRepo)
    {
        bool exists = await usersRepo.Exists(userId);
        if (!exists)
        {
            return Results.BadRequest($"The user with Id '{userId}' does not exist in database");
        }

        FormalEducation? result = await educationRepo.Get(id);
        if (result is null)
        {
            return Results.BadRequest("The entity does not exist in the database.");
        }

        return Results.Ok(result);
    }

    private async Task<IResult> GetAll(
        Guid userId,
        [FromServices] IRepository<FormalEducation> repository)
    {
        List<FormalEducation> list = await repository.List(userId);
        return Results.Ok(list);
    }

    private async Task<IResult> Update(
        Guid userId,
        Guid id,
        FormalEducationUpdateValidator validator,
        [FromBody] FormalEducation entity,
        [FromServices] IRepository<FormalEducation> repository)
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

        FormalEducation newEntity = (FormalEducation)entity.Clone();
        newEntity.UserId = userId;

        FormalEducation? result = await repository.Update(newEntity);
        if (result is null)
        {
            return Results.BadRequest($"Error updating the entity with the Id '{id}'.");
        }

        return Results.NoContent();
    }
}