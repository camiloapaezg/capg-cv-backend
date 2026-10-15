namespace capg_hv_backend.Endpoints.Entities;

public record FileDownloadResultDto(string FileName, byte[] Data);
