using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests.Application;

[Collection("Application services")]
public class FileMetaDataRepositoryShould(ApplicationFixture fixture)
{
    private readonly ApplicationFixture _fixture = fixture;

    private FileMetaData? _metadata;

    private User? _user;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_metadata);
        IListableRepository<FileMetaData> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<FileMetaData>>();
        Assert.NotNull(sut);

        // Creates
        FileMetaData? entity = await sut.CreateAsync(_metadata, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts.
        entity = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        List<FileMetaData> all = await sut.List(_user.Id, TestContext.Current.CancellationToken);
        Assert.Contains(entity, all);

        // Checks result when Owner Id is empty.
        all = await sut.List(Guid.Empty, TestContext.Current.CancellationToken);
        Assert.Empty(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_metadata);
        IListableRepository<FileMetaData> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<FileMetaData>>();
        Assert.NotNull(sut);
        FileMetaData? entity = await sut.CreateAsync(_metadata, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);
        entity = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Deletes
        entity = await sut.DeleteAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts
        entity = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.Null(entity);
    }

    [Fact]
    public async Task UpdateEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_metadata);
        IListableRepository<FileMetaData> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<FileMetaData>>();
        Assert.NotNull(sut);
        FileMetaData? updated = await sut.CreateAsync(_metadata, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.ReadAsync(updated.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        string edit = "Edited";
        updated.Md5Hash = edit;
        updated = await sut.UpdateAsync(updated, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        // Asserts.
        FileMetaData? existing = await sut.ReadAsync(updated.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Md5Hash);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        IListableRepository<User> repository = _fixture.TestHost.Services.GetRequiredService<IListableRepository<User>>();
        _user = await repository.CreateAsync(ApplicationFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
        _metadata = new FileMetaData()
        {
            Name = "file.png",
            SizeInBytes = 1024 * 1024,
            Md5Hash = "filemd5hash",
            UserId = _user.Id,
            ModifiedAt = DateTime.UtcNow,
        }; ;
    }
}