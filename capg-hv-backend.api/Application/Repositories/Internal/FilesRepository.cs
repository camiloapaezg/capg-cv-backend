using Amazon.S3.Model;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Repositories.Entities;
using capg_hv_backend.Infrastructure.FileStorage.Abstractions;
using Microsoft.Extensions.Options;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class FilesRepository(IOptions<RepositoriesOptions> options, IFileStorageClient storageClient) : IFilesRepository
{
    private readonly RepositoriesOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    private readonly IFileStorageClient _storageClient = storageClient ?? throw new ArgumentNullException(nameof(storageClient));

    public async Task<FileOperationResult<object>> DeleteAsync(Guid fileId, CancellationToken token = default)
    {
        return await _storageClient.DeleteAsync(_options.FilesBucketName, fileId.ToString(), token);
    }

    public async Task<FileOperationResult<object>> DeleteFromQuarantineAsync(Guid fileId, CancellationToken token = default)
    {
        return await _storageClient.DeleteAsync(_options.QuarantineBucketName, fileId.ToString(), token);
    }

    public async Task<FileOperationResult<byte[]>> DownloadAsync(Guid fileId, CancellationToken token = default)
    {
        return await _storageClient.DownloadAsync(_options.FilesBucketName, fileId.ToString(), token);
    }

    public async Task<FileOperationResult<byte[]>> DownloadFromQuarantineAsync(Guid fileId, CancellationToken token = default)
    {
        return await _storageClient.DownloadAsync(_options.QuarantineBucketName, fileId.ToString(), token);
    }

    public async Task<FileOperationResult<GetObjectMetadataResponse>> GetMetadataAsync(Guid fileId, CancellationToken token = default)
    {
        return await _storageClient.GetMetadataAsync(_options.FilesBucketName, fileId.ToString(), token);
    }

    public async Task<FileOperationResult<string>> UploadAsync(Guid fileId, Stream fileStream, CancellationToken token = default)
    {
        return await _storageClient.UploadAsync(_options.FilesBucketName, fileId.ToString(), fileStream, token);
    }

    public async Task<FileOperationResult<string>> UploadToQuarantineAsync(Guid fileId, Stream fileStream, CancellationToken token = default)
    {
        return await _storageClient.UploadAsync(_options.QuarantineBucketName, fileId.ToString(), fileStream, token);
    }
}