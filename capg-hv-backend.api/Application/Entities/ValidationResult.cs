namespace capg_hv_backend.Application.Entities;

public sealed record ValidationResult(bool IsValid = true, string? Message = null);
