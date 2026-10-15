using Amazon.S3.Model;
using capg_hv_backend.Application.Repositories.Entities;

namespace capg_hv_backend.Application.Repositories.Abstractions;

public interface IFilesRepository
{
    Task<FileOperationResult<object>> DeleteAsync(Guid fileId, CancellationToken token = default);

    Task<FileOperationResult<object>> DeleteFromQuarantineAsync(Guid fileId, CancellationToken token = default);

    Task<FileOperationResult<byte[]>> DownloadAsync(Guid fileId, CancellationToken token = default);

    Task<FileOperationResult<byte[]>> DownloadFromQuarantineAsync(Guid fileId, CancellationToken token = default);

    Task<FileOperationResult<GetObjectMetadataResponse>> GetMetadataAsync(Guid fileId, CancellationToken token = default);

    Task<FileOperationResult<string>> UploadAsync(Guid fileId, Stream fileStream, CancellationToken token = default);

    Task<FileOperationResult<string>> UploadToQuarantineAsync(Guid fileId, Stream fileStream, CancellationToken token = default);
}