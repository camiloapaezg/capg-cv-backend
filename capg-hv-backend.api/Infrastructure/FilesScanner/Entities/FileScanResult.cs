namespace capg_hv_backend.Infrastructure.FilesScanner.Entities;

public sealed record FileScanResult(FileScanStatus Status, string? Message = null);
