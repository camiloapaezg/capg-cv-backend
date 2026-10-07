namespace capg_hv_backend.Application.Channels.Entities;

public sealed record FileDeleteRequestDto(Guid FileId, string FileName);
