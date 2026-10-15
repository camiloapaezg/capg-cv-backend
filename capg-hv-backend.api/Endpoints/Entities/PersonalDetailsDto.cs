namespace capg_hv_backend.Endpoints.Entities;

public sealed record PersonalDetailsDto(DateTime BirthDate, string? Location, string? Nationality, string? TelephoneNumber);
