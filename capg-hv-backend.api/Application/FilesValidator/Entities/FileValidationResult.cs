namespace capg_hv_backend.Application.FilesValidator.Entities;

public sealed record FileValidationResult(bool IsValid = true, string? Message = null);
