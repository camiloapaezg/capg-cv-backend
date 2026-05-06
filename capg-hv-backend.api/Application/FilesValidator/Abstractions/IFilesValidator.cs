using capg_hv_backend.Application.FilesValidator.Entities;

namespace capg_hv_backend.Application.FilesValidator.Abstractions;

public interface IFilesValidator
{
    Task<ValidationResult> ValidateFileAsync(byte[] content, string fileName, CancellationToken token = default);
}
