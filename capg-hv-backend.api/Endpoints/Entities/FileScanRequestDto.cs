namespace capg_hv_backend.Endpoints.Entities;

public sealed record FileScanRequestDto(Guid Id, Guid UserId, string FileName);
