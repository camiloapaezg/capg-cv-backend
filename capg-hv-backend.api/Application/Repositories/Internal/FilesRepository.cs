using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Repositories.Entities;
using Microsoft.Extensions.Options;
using System.Net;

namespace capg_hv_backend.Application.Repositories.Internal;

public sealed class FilesRepository(IOptions<FileStorageOptions> options) : IFilesRepository
{
    private readonly FileStorageOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    public async Task<FileOperationResult<object>> Delete(Guid fileId, CancellationToken token = default)
    {
        return await Delete(_options.BucketName, fileId.ToString(), token);
    }

    public async Task<FileOperationResult<object>> DeleteFromQuarantine(Guid fileId, CancellationToken token = default)
    {
        return await Delete(_options.QuarantineBucketName, fileId.ToString(), token);
    }

    public async Task<FileOperationResult<byte[]>> Download(Guid fileId, CancellationToken token = default)
    {
        return await Download(_options.BucketName, fileId.ToString(), token);
    }

    public async Task<FileOperationResult<byte[]>> DownloadFromQuarantine(Guid fileId, CancellationToken token = default)
    {
        return await Download(_options.QuarantineBucketName, fileId.ToString(), token);
    }

    public async Task<FileOperationResult<GetObjectMetadataResponse>> GetMetadata(Guid fileId)
    {
        try
        {
            using AmazonS3Client client = CreateS3Client();
            GetObjectMetadataRequest request = new()
            {
                BucketName = _options.BucketName,
                Key = fileId.ToString(),
            };

            GetObjectMetadataResponse response = await client.GetObjectMetadataAsync(request);
            return new FileOperationResult<GetObjectMetadataResponse>(response.HttpStatusCode, response);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return new FileOperationResult<GetObjectMetadataResponse>(ex.StatusCode, Message: ex.Message);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<FileOperationResult<string>> Upload(Guid fileId, Stream fileStream, CancellationToken token = default)
    {
        return await Upload(_options.BucketName, fileId.ToString(), fileStream, token);
    }

    public async Task<FileOperationResult<string>> UploadToQuarantine(Guid fileId, Stream fileStream, CancellationToken token = default)
    {
        return await Upload(_options.QuarantineBucketName, fileId.ToString(), fileStream, token);
    }

    private AmazonS3Client CreateS3Client()
    {
        BasicAWSCredentials credentials = new(_options.AccessKey, _options.SecretKey);
        AmazonS3Config config = new()
        {
            ServiceURL = $"http://{_options.Hostname}:{_options.Port}",
            ForcePathStyle = true,
        };

        return new AmazonS3Client(credentials, config);
    }

    private async Task<FileOperationResult<object>> Delete(string bucketName, string key, CancellationToken token = default)
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

    private async Task<FileOperationResult<byte[]>> Download(string bucketName, string key, CancellationToken token = default)
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

    private async Task<FileOperationResult<string>> Upload(string bucketName, string key, Stream fileStream, CancellationToken token = default)
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
            MD5Digest = md5Checksum,
            CalculateContentMD5Header = true,
        };

        putRequest.Metadata.Add("x-amz-meta-md5", md5Checksum);

        PutObjectResponse response = await client.PutObjectAsync(putRequest, token);
        return new FileOperationResult<string>(response.HttpStatusCode, md5Checksum);
    }
}