namespace capg_hv_backend.InterfaceAdapters.Entities;

public sealed record HttpErrorDetails(int StatusCode, string? Message = null, string? Details = null);
