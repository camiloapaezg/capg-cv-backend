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
        FileOperationResult<string> uploadResult = await sut.Upload(fileId, stream);
        Assert.Equal(HttpStatusCode.OK, uploadResult.StatusCode);
        Assert.NotNull(uploadResult.Data);

        // Gets info
        FileOperationResult<GetObjectMetadataResponse> getInfoResult = await sut.GetMetadata(fileId);
        Assert.Equal(HttpStatusCode.OK, getInfoResult.StatusCode);
        Assert.True(getInfoResult.Data is not null);

        // Downloads
        FileOperationResult<byte[]> downloadResult = await sut.Download(fileId);
        if (downloadResult?.Data is null)
        {
            Assert.Fail("The result does not contain data");
        }

        Assert.Equal(HttpStatusCode.OK, downloadResult.StatusCode);
        Assert.Equal(TestFiles.JPG, downloadResult.Data);

        // Deletes
        FileOperationResult<object> deleteResult = await sut.Delete(fileId);
        Assert.Equal(HttpStatusCode.NoContent, deleteResult.StatusCode);

        // Checkes
        getInfoResult = await sut.GetMetadata(fileId);
        Assert.Equal(HttpStatusCode.NotFound, getInfoResult.StatusCode);
    }
}