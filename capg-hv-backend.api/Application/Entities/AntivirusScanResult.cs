namespace capg_hv_backend.Application.Entities;

public sealed record AntivirusScanResult(AntivirusScanStatus Status, string? Message = null);
