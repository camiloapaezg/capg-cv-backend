namespace capg_hv_backend.Endpoints.Entities;

public sealed record UserDto(
    string FirstName,
    string LastName,
    string EmailAddress,
    DateTime BirthDate,
    string? Location,
    string? Nationality,
    string? TelephoneNumber);