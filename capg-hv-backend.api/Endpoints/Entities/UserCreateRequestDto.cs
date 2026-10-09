namespace capg_hv_backend.Endpoints.Entities;

public sealed record UserCreateRequestDto(
    string FirstName,
    string LastName,
    string EmailAddress,
    string? TelephoneNumber,
    DateTime? BirthDate,
    string? Nationality);