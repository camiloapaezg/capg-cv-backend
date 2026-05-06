namespace capg_hv_backend.Application.Middlewares.Entities;

public sealed record HttpErrorDetails(int StatusCode, string? Message = null, string? Details = null);
