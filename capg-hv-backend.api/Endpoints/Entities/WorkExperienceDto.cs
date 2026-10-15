namespace capg_hv_backend.Endpoints.Entities;

public sealed record WorkExperienceDto(string Company,
                                       string JobTitle,
                                       string Location,
                                       string Description,
                                       DateTime From,
                                       DateTime? Until,
                                       string? ContactName,
                                       string? ContactNumber);
