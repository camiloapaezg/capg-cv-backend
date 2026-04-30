using System.Net;

namespace capg_hv_backend.Application.Entities;

public sealed record FileOperationResult<T>(HttpStatusCode StatusCode, T? Data = default, string? Message = null);
