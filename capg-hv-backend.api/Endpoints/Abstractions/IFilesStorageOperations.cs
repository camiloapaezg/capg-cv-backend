using capg_hv_backend.Endpoints.Entities;

namespace capg_hv_backend.Endpoints.Abstractions;

public interface IFilesStorageOperations
{
    Task<FileHttpOperationResultDto<FileDownloadResultDto>> DownloadFileAsync(Guid fileId, CancellationToken token = default);

    Task<FileHttpOperationResultDto<FileScanRequestDto>> StartUploadFileAsync(Guid userId, IFormFile file, CancellationToken token = default);
}
