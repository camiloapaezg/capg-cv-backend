using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Validators.Internal;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Endpoints.Entities;
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
        FormalEducationValidator validator,
        [FromBody] FormalEducationDto request,
        [FromServices] IListableRepository<User> userRepository,
        [FromServices] IListableRepository<FormalEducation> educationRepository)
    {
        // Validates request.
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

        // Prepares and creates entry
        FormalEducation? newEntity = new()
        {
            Degree = request.Degree,
            Institution = request.Institution,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = request.Description
        };

        newEntity = await educationRepository.CreateAsync(newEntity);
        if (newEntity is null)
        {
            return Results.InternalServerError($"Error creating formal education entry for the user with Id '{userId}'.");
        }

        return Results.Created($"api/v1/users/{userId}/education/{newEntity.Id}", newEntity);
    }

    private async Task<IResult> Delete(
        Guid userId,
        Guid id,
        [FromServices] IListableRepository<FormalEducation> repository)
    {
        FormalEducation? entry = await repository.ReadAsync(id);
        if (entry is null)
        {
            return Results.BadRequest($"The formal education entry with Id '{id}' related to the user with Id '{userId}' was not found.");
        }

        if (entry.UserId != userId)
        {
            return Results.BadRequest("The formal education entry user Id does not correspond to the one read from the request.");
        }

        FormalEducation? result = await repository.DeleteAsync(id);
        if (result is null)
        {
            return Results.InternalServerError($"Error deleting formal education entry with Id '{id}' related to the user with id '{userId}'.");
        }

        return Results.NoContent();
    }

    private async Task<IResult> Get(
        Guid userId,
        Guid id,
        [FromServices] IListableRepository<FormalEducation> repository)
    {
        FormalEducation? existing = await repository.ReadAsync(id);
        if (existing is null)
        {
            return Results.BadRequest($"The formal education entry with Id '{id}' related to the user with Id '{userId}' was not found.");
        }

        if (existing.UserId != userId)
        {
            return Results.BadRequest("The entry user Id does not correspond to the one read from the request.");
        }

        return Results.Ok(existing);
    }

    private async Task<IResult> GetAll(
        Guid userId,
        [FromServices] IListableRepository<FormalEducation> repository)
    {
        List<FormalEducation> list = await repository.List(userId);
        return Results.Ok(list);
    }

    private async Task<IResult> Update(
        Guid userId,
        Guid id,
        FormalEducationValidator validator,
        [FromBody] FormalEducationDto request,
        [FromServices] IListableRepository<FormalEducation> repository)
    {
        ValidationResult validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Results.UnprocessableEntity(validationResult.GetFormattedErrors());
        }

        FormalEducation? existing = await repository.ReadAsync(id);
        if (existing is null)
        {
            return Results.BadRequest($"The formal education entry with Id '{id}' related to the user with Id '{userId}' was not found.");
        }

        if (existing.UserId != userId)
        {
            return Results.BadRequest("The entry user Id does not correspond to the one read from the request.");
        }

        FormalEducation? newEntity = (FormalEducation)existing.Clone();
        newEntity.Degree = request.Degree;
        newEntity.Institution = request.Institution;
        newEntity.StartDate = request.StartDate;
        newEntity.EndDate = request.EndDate;
        newEntity.Description = request.Description;

        newEntity = await repository.UpdateAsync(newEntity);
        if (newEntity is null)
        {
            return Results.InternalServerError($"Error updating the formal education entry with the Id '{id}' related to the user with Id '{userId}'..");
        }

        return Results.NoContent();
    }
}