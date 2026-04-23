using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests;

[Collection("Repositories collection")]
public class CertificationTrainingRepositoryShould(RepositoriesFixture fixture)
{
    private readonly RepositoriesFixture _fixture = fixture;

    private CertificationTraining? _certification;

    private User? _user;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_certification);
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<CertificationTraining>>();
        Assert.NotNull(sut);

        // Creates
        var created = await sut.Create(_certification);
        Assert.NotNull(created);

        // Asserts.
        var existing = await sut.Get(created.Id);
        Assert.NotNull(existing);

        var all = await sut.List(_certification.UserId);
        Assert.Single(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_certification);
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<CertificationTraining>>();
        Assert.NotNull(sut);
        CertificationTraining? deleted = await sut.Create(_certification);
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
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_certification);
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<CertificationTraining>>();
        Assert.NotNull(sut);
        CertificationTraining? updated = await sut.Create(_certification);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.Get(updated.Id);
        Assert.NotNull(updated);

        var edit = "Edited";
        updated.Institution = new string(edit);
        updated = await sut.Update(updated);
        Assert.NotNull(updated);

        // Asserts.
        var existing = await sut.Get(updated.Id);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Institution);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        var usersRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        _user = await usersRepo.Create(RepositoriesFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
        _certification = new CertificationTraining()
        {
            Title = $"Certification Title",
            Institution = "Certification Institution",
            FinishedAt = "2012.04",
            UserId = _user.Id,
        };
    }
}