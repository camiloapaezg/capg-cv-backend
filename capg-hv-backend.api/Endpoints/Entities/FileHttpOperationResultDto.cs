using System.Net;

namespace capg_hv_backend.Endpoints.Entities;

public sealed record FileHttpOperationResultDto<TValue>(HttpStatusCode Status, string? Message = null, TValue? Value = default);
