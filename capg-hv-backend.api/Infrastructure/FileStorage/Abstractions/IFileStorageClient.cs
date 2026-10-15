using Amazon.S3.Model;
using capg_hv_backend.Application.Repositories.Entities;

namespace capg_hv_backend.Infrastructure.FileStorage.Abstractions;

public interface IFileStorageClient
{
    Task<FileOperationResult<object>> DeleteAsync(string bucketName, string key, CancellationToken token = default);

    Task<FileOperationResult<byte[]>> DownloadAsync(string bucketName, string key, CancellationToken token = default);

    Task<FileOperationResult<GetObjectMetadataResponse>> GetMetadataAsync(string bucketName, string key, CancellationToken token = default);

    Task<FileOperationResult<string>> UploadAsync(string bucketName, string key, Stream fileStream, CancellationToken token = default);
}