using System.Net;

namespace capg_hv_backend.Application.Repositories.Entities;

public sealed record FileOperationResult<T>(HttpStatusCode StatusCode, T? Data = default, string? Message = null);
