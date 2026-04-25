using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests;

[Collection("Repositories collection")]
public class PublicationRepositoryShould(RepositoriesFixture fixture)
{
    private readonly RepositoriesFixture _fixture = fixture;

    private Publication? _publication;

    private User? _user;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_publication);
        IRepository<Publication> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<Publication>>();
        Assert.NotNull(sut);

        // Creates
        Publication? created = await sut.Create(_publication);
        Assert.NotNull(created);

        // Asserts.
        bool exists = await sut.Exists(created.Id);
        Assert.True(exists);

        List<Publication> all = await sut.List(_publication.UserId);
        Assert.Single(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_publication);
        IRepository<Publication> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<Publication>>();
        Assert.NotNull(sut);
        Publication? deleted = await sut.Create(_publication);
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
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_publication);
        IRepository<Publication> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<Publication>>();
        Assert.NotNull(sut);
        Publication? updated = await sut.Create(_publication);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.Get(updated.Id);
        Assert.NotNull(updated);

        string edit = "Edited";
        updated.Location = new string(edit);
        updated = await sut.Update(updated);
        Assert.NotNull(updated);

        // Asserts.
        Publication? existing = await sut.Get(updated.Id);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Location);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        IRepository<User> usersRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        _user = await usersRepo.Create(RepositoriesFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
        _publication = new Publication()
        {
            Title = "Publication Title",
            Type = "Type",
            Location = "Germany",
            Doi = "123456",
            PublishedAt = "2012.04",
            UserId = _user.Id,
        };
    }
}