using capg_hv_backend.Application.Channels.Abstractions;
using capg_hv_backend.Application.Channels.Entities;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Validators.Internal;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Endpoints.Entities;
using Carter;
using Carter.ModelBinding;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace capg_hv_backend.Endpoints.Internal;

public sealed class UserEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("api/v1/users")
            .WithTags("Users");

        group.MapGet("/", ReadAll)
            .Produces<List<User>>()
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("{id:guid}", Read)
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
    }

    private async Task<IResult> Create(
        UserValidator validator,
        [FromBody] UserDto request,
        [FromServices] IListableRepository<User> usersRepository,
        [FromServices] ICrudRepository<GeneralDetails> generalDetailsRepository,
        [FromServices] ICrudRepository<PersonalDetails> personalDetailsRepository)
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

        newUser = await usersRepository.CreateAsync(newUser);
        if (newUser is null)
        {
            return Results.InternalServerError($"Error creating user with email address '{request.EmailAddress}'.");
        }

        // Creates general and project details
        GeneralDetails? generalDetails = new()
        {
            Title = $"{newUser.FirstName} {newUser.LastName}",
            UserId = newUser.Id
        };

        generalDetails = await generalDetailsRepository.CreateAsync(generalDetails);
        if (generalDetails is null)
        {
            await usersRepository.DeleteAsync(newUser.Id);
            return Results.InternalServerError($"Error creating general details for the user with email address '{newUser.EmailAddress}'.");
        }

        PersonalDetails? personalDetails = new()
        {
            BirthDate = request.BirthDate,
            Nationality = request.Nationality,
            TelephoneNumber = request.TelephoneNumber,
            Location = request.Location,
            UserId = newUser.Id
        };

        personalDetails = await personalDetailsRepository.CreateAsync(personalDetails);
        if (personalDetails is null)
        {
            await usersRepository.DeleteAsync(newUser.Id);
            return Results.InternalServerError($"Error creating personal details for the user with email address '{newUser.EmailAddress}'.");
        }

        return Results.Created($"api/v1/users/{newUser.Id}", newUser);
    }

    private async Task<IResult> Delete(Guid id,
        [FromServices] IListableRepository<User> usersRepository,
        [FromServices] IListableRepository<FileMetaData> metadataRepository,
        [FromServices] IChannel<FileDeleteRequestDto> channel)
    {
        // Loads files metadata prior to user removal.
        List<FileMetaData> files = await metadataRepository.List(id);

        // Deletes user
        User? result = await usersRepository.DeleteAsync(id);
        if (result is null)
        {
            return Results.BadRequest($"Error deleting the user with Id '{id}'.");
        }

        // Sends request for files removal to hosted service.
        if (files.Count > 0)
        {
            files.ForEach(async file => await channel.WriteAsync(new FileDeleteRequestDto(id, file.Id, file.Name)));
        }

        return Results.Accepted();
    }

    private async Task<IResult> Read(Guid id, [FromServices] IListableRepository<User> repository)
    {
        User? result = await repository.ReadAsync(id);
        if (result is null)
        {
            return Results.BadRequest($"The user with Id '{id}' does not exist in the database.");
        }

        return Results.Ok(result);
    }

    private async Task<IResult> ReadAll([FromServices] IListableRepository<User> repository)
    {
        List<User> list = await repository.List();
        return Results.Ok(list);
    }

    private async Task<IResult> Update(
        Guid id,
        UserValidator validator,
        [FromBody] UserDto request,
        [FromServices] IListableRepository<User> userRepository,
        [FromServices] ICrudRepository<PersonalDetails> personalDetailsRepository)
    {
        ValidationResult validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Results.UnprocessableEntity(validationResult.GetFormattedErrors());
        }

        // Gets the entities to update.
        User? user = await userRepository.ReadAsync(id);
        PersonalDetails? details = await personalDetailsRepository.ReadAsync(id);
        if (user is null || details is null)
        {
            string missingEntry = user is null ? "user" : "details";
            return Results.BadRequest($"The {missingEntry} does not exist in the database.");
        }

        // Updates entities
        User? newUser = (User)user.Clone();
        newUser.FirstName = request.FirstName;
        newUser.LastName = request.LastName;
        newUser.EmailAddress = request.EmailAddress;

        PersonalDetails? newDetails = (PersonalDetails)details.Clone();
        newDetails.BirthDate = request.BirthDate;
        newDetails.Nationality = request.Nationality;
        newDetails.TelephoneNumber = request.TelephoneNumber;
        newDetails.Location = request.Location;

        newUser = await userRepository.UpdateAsync(newUser);
        if (newUser is null)
        {
            return Results.BadRequest($"Error updating the user with the Id '{id}'.");
        }

        newDetails = await personalDetailsRepository.UpdateAsync(newDetails);
        if (newDetails is null)
        {
            return Results.BadRequest($"Error updating the personal details of the user with Id '{id}'.");
        }

        return Results.NoContent();
    }
}