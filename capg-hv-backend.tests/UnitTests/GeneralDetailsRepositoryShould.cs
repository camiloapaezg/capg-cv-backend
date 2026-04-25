using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests;

[Collection("Repositories collection")]
public class GeneralDetailsRepositoryShould(RepositoriesFixture fixture)
{
    private readonly RepositoriesFixture _fixture = fixture;

    private GeneralDetails? _generalDetails;

    private User? _user;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_generalDetails);
        IRepository<GeneralDetails> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<GeneralDetails>>();
        Assert.NotNull(sut);

        // Creates
        GeneralDetails? created = await sut.Create(_generalDetails);
        Assert.NotNull(created);

        // Asserts.
        bool exists = await sut.Exists(created.Id);
        Assert.True(exists);

        List<GeneralDetails> all = await sut.List(_generalDetails.UserId);
        Assert.Single(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_generalDetails);
        IRepository<GeneralDetails> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<GeneralDetails>>();
        Assert.NotNull(sut);
        GeneralDetails? deleted = await sut.Create(_generalDetails);
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
        Assert.NotNull(_generalDetails);
        IRepository<GeneralDetails> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<GeneralDetails>>();
        Assert.NotNull(sut);
        GeneralDetails? updated = await sut.Create(_generalDetails);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.Get(updated.Id);
        Assert.NotNull(updated);

        string edit = "Edited";
        updated.Description = new string(edit);
        updated = await sut.Update(updated);
        Assert.NotNull(updated);

        // Asserts.
        GeneralDetails? existing = await sut.Get(updated.Id);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Description);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        IRepository<User> usersRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        _user = await usersRepo.Create(RepositoriesFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
        _generalDetails = new GeneralDetails()
        {
            UserId = _user.Id,
            Title = "Title"
        };
    }
}