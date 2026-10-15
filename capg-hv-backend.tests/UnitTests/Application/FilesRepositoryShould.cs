using Amazon.S3;
using Amazon.S3.Model;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Repositories.Entities;
using capg_hv_backend.tests.Fixtures;
using capg_hv_backend.tests.Resources;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace capg_hv_backend.tests.UnitTests.Application;

[Collection("Application services")]
public class FilesRepositoryShould(ApplicationFixture fixture)
{
    private readonly ApplicationFixture _fixture = fixture;

    [Fact]
    public async Task UploadDownloadAndDeleteFile()
    {
        IFilesRepository sut = _fixture.TestHost.Services.GetRequiredService<IFilesRepository>();
        Assert.NotNull(sut);

        // Uploads
        Guid fileId = Guid.NewGuid();
        using MemoryStream stream = new(TestFiles.JPG);
        FileOperationResult<string> uploadResult = await sut.UploadAsync(fileId, stream, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, uploadResult.StatusCode);
        Assert.NotNull(uploadResult.Data);

        // Gets info
        FileOperationResult<GetObjectMetadataResponse> getInfoResult = await sut.GetMetadataAsync(fileId, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, getInfoResult.StatusCode);
        Assert.True(getInfoResult.Data is not null);

        // Downloads
        FileOperationResult<byte[]> downloadResult = await sut.DownloadAsync(fileId, TestContext.Current.CancellationToken);
        if (downloadResult?.Data is null)
        {
            Assert.Fail("The result does not contain data");
        }

        Assert.Equal(HttpStatusCode.OK, downloadResult.StatusCode);
        Assert.Equal(TestFiles.JPG, downloadResult.Data);

        // Deletes
        FileOperationResult<object> deleteResult = await sut.DeleteAsync(fileId, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, deleteResult.StatusCode);

        // Checks that the file does not exist anymore
        await Assert.ThrowsAsync<AmazonS3Exception>(async () => await sut.GetMetadataAsync(fileId, TestContext.Current.CancellationToken));
    }
}