using Amazon.S3.Model;
using capg_hv_backend.Application.Entities;

namespace capg_hv_backend.Application.Repositories.Abstractions;

public interface IFilesRepository
{
    Task<FileOperationResult<object>> DeleteBucket(string bucketName, CancellationToken token = default);

    Task<FileOperationResult<object>> DeleteFile(string bucketName, string key, CancellationToken token = default);

    Task<FileOperationResult<byte[]>> DownloadFile(string bucketName, string key, CancellationToken token = default);

    Task<FileOperationResult<GetObjectMetadataResponse>> GetFileInformation(string bucketName, string key);

    Task<FileOperationResult<object>> UploadFile(string bucketName, string key, Stream fileStream, CancellationToken token = default);
}