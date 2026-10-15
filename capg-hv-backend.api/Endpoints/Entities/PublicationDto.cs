namespace capg_hv_backend.Endpoints.Entities;

public sealed record PublicationDto(string Title, string Type, string Location, string PublishedAt, string? Doi);