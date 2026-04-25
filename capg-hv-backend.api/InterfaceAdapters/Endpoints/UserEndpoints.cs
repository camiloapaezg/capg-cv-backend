using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Validators.Internal;
using capg_hv_backend.Domain.Entities;
using Carter;
using Carter.ModelBinding;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace capg_hv_backend.InterfaceAdapters.Endpoints;

public sealed class UserEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("api/v1/users");

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
        UserAddValidator validator,
        [FromBody] User entity,
        [FromServices] IRepository<User> repository)
    {
        ValidationResult validationResult = validator.Validate(entity);
        if (!validationResult.IsValid)
        {
            return Results.UnprocessableEntity(validationResult.GetFormattedErrors());
        }

        User? result = await repository.Create(entity);
        if (result is null)
        {
            return Results.Problem("Error creating user.", statusCode: 500);
        }

        return Results.Created($"api/v1/users/{result.Id}", result);
    }

    private async Task<IResult> Delete(Guid id, [FromServices] IRepository<User> repository)
    {
        User? result = await repository.Delete(id);
        if (result is null)
        {
            return Results.BadRequest($"Error deleting the user with Id '{id}'");
        }

        return Results.NoContent();
    }

    private async Task<IResult> Get(Guid id, [FromServices] IRepository<User> repository)
    {
        User? result = await repository.Get(id);
        if (result is null)
        {
            return Results.BadRequest("The user does not exist in the database.");
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
            return Results.BadRequest($"Error updating the user with the Id '{id}'.");
        }

        return Results.NoContent();
    }
}