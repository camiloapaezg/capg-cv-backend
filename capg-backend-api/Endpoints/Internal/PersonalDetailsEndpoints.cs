using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Validators.Internal;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Endpoints.Entities;
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

        group.MapGet("/", Read)
            .Produces<PersonalDetails>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/", Update)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private async Task<IResult> Read(
        Guid userId,
        [FromServices] ICrudRepository<PersonalDetails> repository)
    {
        PersonalDetails? result = await repository.ReadAsync(userId);
        if (result is null)
        {
            return Results.BadRequest($"The personal details entry of the user with Id '{userId}' does not exist in the database.");
        }

        return Results.Ok(result);
    }

    private async Task<IResult> Update(
        Guid userId,
        PersonalDetailsValidator validator,
        [FromBody] PersonalDetailsDto request,
        [FromServices] ICrudRepository<PersonalDetails> repository)
    {
        // Validates the request
        ValidationResult validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Results.UnprocessableEntity(validationResult.GetFormattedErrors());
        }

        PersonalDetails? existing = await repository.ReadAsync(userId);
        if (existing is null)
        {
            return Results.BadRequest($"The personal details entry of the user with Id '{userId}' does not exist in the database.");
        }

        // Updates the entry.
        PersonalDetails? newEntry = (PersonalDetails)existing.Clone();
        newEntry.BirthDate = request.BirthDate;
        newEntry.Location = request.Location;
        newEntry.Nationality = request.Nationality;
        newEntry.TelephoneNumber = request.TelephoneNumber;

        newEntry = await repository.UpdateAsync(newEntry);
        if (newEntry is null)
        {
            return Results.BadRequest($"Error updating the personal details of the user with Id '{userId}'.");
        }

        return Results.NoContent();
    }
}