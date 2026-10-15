namespace capg_hv_backend.Endpoints.Entities;

public sealed record FormalEducationDto(string Degree, string Institution, string StartDate, string? EndDate, string? Description);
