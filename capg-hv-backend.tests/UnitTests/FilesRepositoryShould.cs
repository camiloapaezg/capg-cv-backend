using Amazon.S3.Model;
using capg_hv_backend.Application.Entities;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.tests.Fixtures;
using capg_hv_backend.tests.Resources;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace capg_hv_backend.tests.UnitTests;

[Collection("Repositories collection")]
public class FilesRepositoryShould(RepositoriesFixture fixture)
{
    private readonly RepositoriesFixture _fixture = fixture;

    [Fact]
    public async Task UploadDownloadAndDeleteFile()
    {
        IFilesRepository sut = _fixture.TestHost.Services.GetRequiredService<IFilesRepository>();
        Assert.NotNull(sut);

        // Uploads
        Guid fileId = Guid.NewGuid();
        using MemoryStream stream = new(TestFiles.JPG);
        FileOperationResult<object> uploadResult = await sut.UploadFile(RepositoriesFixture.BucketName, fileId.ToString(), stream);
        Assert.Equal(HttpStatusCode.OK, uploadResult.StatusCode);

        // Gets info
        FileOperationResult<GetObjectMetadataResponse> getInfoResult = await sut.GetFileInformation(RepositoriesFixture.BucketName, fileId.ToString());
        Assert.Equal(HttpStatusCode.OK, getInfoResult.StatusCode);
        Assert.True(getInfoResult.Data is not null);

        // Downloads
        FileOperationResult<byte[]> downloadResult = await sut.DownloadFile(RepositoriesFixture.BucketName, fileId.ToString());
        if (downloadResult?.Data is null)
        {
            Assert.Fail("The result does not contain data");
        }

        Assert.Equal(HttpStatusCode.OK, downloadResult.StatusCode);
        Assert.Equal(TestFiles.JPG, downloadResult.Data);

        // Deletes
        FileOperationResult<object> deleteResult = await sut.DeleteFile(RepositoriesFixture.BucketName, fileId.ToString());
        Assert.Equal(HttpStatusCode.NoContent, deleteResult.StatusCode);

        // Checkes
        getInfoResult = await sut.GetFileInformation(RepositoriesFixture.BucketName, fileId.ToString());
        Assert.Equal(HttpStatusCode.NotFound, getInfoResult.StatusCode);
    }
}