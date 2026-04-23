namespace capg_hv_backend.Application.Entities;

public sealed record ValidationResult(bool IsValid, string? Message = null);
