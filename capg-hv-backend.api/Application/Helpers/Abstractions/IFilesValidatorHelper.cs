using capg_hv_backend.Application.Entities;

namespace capg_hv_backend.Application.Helpers.Abstractions;

public interface IFilesValidatorHelper
{
    Task<ValidationResult> ValidateFileAsync(byte[] content, string fileName, CancellationToken token = default);
}
