using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using capg_hv_backend.Application.Repositories.Entities;
using capg_hv_backend.Infrastructure.FileStorage.Abstractions;
using Microsoft.Extensions.Options;
using System.Net;

namespace capg_hv_backend.Infrastructure.FileStorage.Internal;

public sealed class FileStorageClient(IOptions<FileStorageOptions> options) : IFileStorageClient
{
    private readonly int _maxErrorRetry = 3;

    private readonly FileStorageOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);

    public async Task<FileOperationResult<object>> DeleteAsync(string bucketName, string key, CancellationToken token = default)
    {
        using AmazonS3Client client = CreateS3Client();

        // Creates the Bucket
        bool bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(client, bucketName);
        if (!bucketExists)
        {
            await client.PutBucketAsync(bucketName, token);
        }

        DeleteObjectRequest deleteRequest = new()
        {
            BucketName = bucketName,
            Key = key,
        };

        DeleteObjectResponse response = await client.DeleteObjectAsync(deleteRequest, token);
        return new FileOperationResult<object>(response.HttpStatusCode);
    }

    public async Task<FileOperationResult<byte[]>> DownloadAsync(string bucketName, string key, CancellationToken token = default)
    {
        using AmazonS3Client client = CreateS3Client();
        bool bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(client, bucketName);
        if (!bucketExists)
        {
            return new FileOperationResult<byte[]>(HttpStatusCode.BadRequest, Message: $"The bucket '{bucketName}' does not exist.");
        }

        GetObjectRequest getObjectRequest = new()
        {
            BucketName = bucketName,
            Key = key,
        };

        using GetObjectResponse response = await client.GetObjectAsync(getObjectRequest, token);
        if (response.HttpStatusCode != HttpStatusCode.OK || response.ResponseStream is null)
        {
            return new FileOperationResult<byte[]>(response.HttpStatusCode, Message: $"Error downloading the file with key '{key}' from the bucket '{bucketName}'.");
        }

        using MemoryStream memoryStream = new();
        await response.ResponseStream.CopyToAsync(memoryStream, token);

        return new FileOperationResult<byte[]>(response.HttpStatusCode, memoryStream.ToArray());
    }

    public async Task<FileOperationResult<GetObjectMetadataResponse>> GetMetadataAsync(string bucketName, string key, CancellationToken token = default)
    {
        using AmazonS3Client client = CreateS3Client();
        GetObjectMetadataRequest request = new()
        {
            BucketName = bucketName,
            Key = key,
        };

        GetObjectMetadataResponse response = await client.GetObjectMetadataAsync(request, token);
        return new FileOperationResult<GetObjectMetadataResponse>(response.HttpStatusCode, response);
    }

    public async Task<FileOperationResult<string>> UploadAsync(string bucketName, string key, Stream fileStream, CancellationToken token = default)
    {
        using AmazonS3Client client = CreateS3Client();

        // Creates the Bucket
        bool bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(client, bucketName);
        if (!bucketExists)
        {
            await client.PutBucketAsync(bucketName, token);
        }

        // Uploads the file as stream.
        string md5Checksum = AmazonS3Util.GenerateMD5ChecksumForStream(fileStream);
        PutObjectRequest putRequest = new()
        {
            BucketName = bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = "application/octet-stream",
            ChecksumMD5 = md5Checksum
        };

        putRequest.Metadata.Add("x-amz-meta-md5", md5Checksum);

        PutObjectResponse response = await client.PutObjectAsync(putRequest, token);
        return new FileOperationResult<string>(response.HttpStatusCode, md5Checksum);
    }

    private AmazonS3Client CreateS3Client()
    {
        BasicAWSCredentials credentials = new(_options.AccessKey, _options.SecretKey);
        AmazonS3Config config = new()
        {
            ServiceURL = $"http://{_options.Hostname}:{_options.Port}",
            ForcePathStyle = true,
            RetryMode = RequestRetryMode.Standard,
            MaxErrorRetry = _maxErrorRetry,
            Timeout = _timeout,
        };

        return new AmazonS3Client(credentials, config);
    }
}