using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests.Application;

[Collection("Application services")]
public class FileMetaDataRepositoryShould(ApplicationFixture fixture)
{
    private static readonly FileMetaData File = new()
    {
        Name = "file.png",
        SizeInBytes = 1024 * 1024,
        Md5Hash = "filemd5hash",
        OwnerId = Guid.NewGuid(),
        ModifiedAt = DateTime.UtcNow,
    };

    private readonly ApplicationFixture _fixture = fixture;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        IRepository<FileMetaData> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<FileMetaData>>();
        Assert.NotNull(sut);

        // Creates
        FileMetaData? created = await sut.Create(File, TestContext.Current.CancellationToken);
        Assert.NotNull(created);

        // Asserts.
        bool exists = await sut.Exists(created.Id, TestContext.Current.CancellationToken);
        Assert.True(exists);

        List<FileMetaData> all = await sut.List(File.OwnerId, TestContext.Current.CancellationToken);
        Assert.Contains(created, all);

        // Checks result when Owner Id is empty.
        all = await sut.List(Guid.Empty, TestContext.Current.CancellationToken);
        Assert.Empty(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        IRepository<FileMetaData> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<FileMetaData>>();
        Assert.NotNull(sut);
        FileMetaData? deleted = await sut.Create(File, TestContext.Current.CancellationToken);
        Assert.NotNull(deleted);
        deleted = await sut.Get(deleted.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(deleted);

        // Deletes
        deleted = await sut.Delete(deleted.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(deleted);

        // Asserts
        bool exists = await sut.Exists(deleted.Id, TestContext.Current.CancellationToken);
        Assert.False(exists);
    }

    [Fact]
    public async Task UpdateEntity()
    {
        // Prepares
        IRepository<FileMetaData> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<FileMetaData>>();
        Assert.NotNull(sut);
        FileMetaData? updated = await sut.Create(File, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.Get(updated.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        string edit = "Edited";
        updated.Md5Hash = new string(edit);
        updated = await sut.Update(updated, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        // Asserts.
        FileMetaData? existing = await sut.Get(updated.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Md5Hash);
    }
}