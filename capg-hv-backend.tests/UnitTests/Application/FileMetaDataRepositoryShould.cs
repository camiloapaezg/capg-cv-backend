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
        FileMetaData? created = await sut.Create(File);
        Assert.NotNull(created);

        // Asserts.
        bool exists = await sut.Exists(created.Id);
        Assert.True(exists);

        List<FileMetaData> all = await sut.List();
        Assert.Contains(created, all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        IRepository<FileMetaData> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<FileMetaData>>();
        Assert.NotNull(sut);
        FileMetaData? deleted = await sut.Create(File);
        Assert.NotNull(deleted);
        deleted = await sut.Get(deleted.Id);
        Assert.NotNull(deleted);

        // Deletes
        deleted = await sut.Delete(deleted.Id);
        Assert.NotNull(deleted);

        // Asserts
        bool exists = await sut.Exists(deleted.Id);
        Assert.False(exists);
    }

    [Fact]
    public async Task UpdateEntity()
    {
        // Prepares
        IRepository<FileMetaData> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<FileMetaData>>();
        Assert.NotNull(sut);
        FileMetaData? updated = await sut.Create(File);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.Get(updated.Id);
        Assert.NotNull(updated);

        string edit = "Edited";
        updated.Md5Hash = new string(edit);
        updated = await sut.Update(updated);
        Assert.NotNull(updated);

        // Asserts.
        FileMetaData? existing = await sut.Get(updated.Id);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Md5Hash);
    }
}