using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests;

[Collection("Repositories collection")]
public class UsersRepositoryShould(RepositoriesFixture fixture)
{
    private readonly RepositoriesFixture _fixture = fixture;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        Assert.NotNull(sut);

        // Creates
        var created = await sut.Create(RepositoriesFixture.DefaultUser);
        Assert.NotNull(created);

        // Asserts.
        var existing = await sut.Get(created.Id);
        Assert.NotNull(existing);

        var all = await sut.List();
        Assert.Single(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        Assert.NotNull(sut);
        User? deleted = await sut.Create(RepositoriesFixture.DefaultUser);
        Assert.NotNull(deleted);
        deleted = await sut.Get(deleted.Id);
        Assert.NotNull(deleted);

        // Deletes
        deleted = await sut.Delete(deleted.Id);
        Assert.NotNull(deleted);

        // Asserts
        deleted = await sut.Get(deleted.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task UpdateEntity()
    {
        // Prepares
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        Assert.NotNull(sut);
        User? updated = await sut.Create(RepositoriesFixture.DefaultUser);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.Get(updated.Id);
        Assert.NotNull(updated);

        var edit = "Edited";
        updated.LastName = new string(edit);
        updated = await sut.Update(updated);
        Assert.NotNull(updated);

        // Asserts.
        var existing = await sut.Get(updated.Id);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.LastName);
    }
}