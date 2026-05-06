using Amazon.S3.Model;
using capg_hv_backend.Application.Repositories.Entities;

namespace capg_hv_backend.Application.Repositories.Abstractions;

public interface IFilesRepository
{
    Task<FileOperationResult<object>> Delete(Guid fileId, CancellationToken token = default);

    Task<FileOperationResult<object>> DeleteFromQuarantine(Guid fileId, CancellationToken token = default);

    Task<FileOperationResult<byte[]>> Download(Guid fileId, CancellationToken token = default);

    Task<FileOperationResult<byte[]>> DownloadFromQuarantine(Guid fileId, CancellationToken token = default);

    Task<FileOperationResult<GetObjectMetadataResponse>> GetMetadata(Guid fileId);

    Task<FileOperationResult<string>> Upload(Guid fileId, Stream fileStream, CancellationToken token = default);

    Task<FileOperationResult<string>> UploadToQuarantine(Guid fileId, Stream fileStream, CancellationToken token = default);
}