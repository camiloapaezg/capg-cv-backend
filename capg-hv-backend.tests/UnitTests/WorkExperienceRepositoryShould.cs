using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests;

[Collection("Repositories collection")]
public class WorkExperienceRepositoryShould(RepositoriesFixture fixture)
{
    private readonly RepositoriesFixture _fixture = fixture;

    private WorkExperience? _experience;

    private User? _user;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_experience);
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<WorkExperience>>();
        Assert.NotNull(sut);

        // Creates
        var created = await sut.Create(_experience);
        Assert.NotNull(created);

        // Asserts.
        var existing = await sut.Get(created.Id);
        Assert.NotNull(existing);

        var all = await sut.List(_experience.UserId);
        Assert.Single(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_experience);
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<WorkExperience>>();
        Assert.NotNull(sut);
        WorkExperience? deleted = await sut.Create(_experience);
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
        Assert.NotNull(_experience);
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<WorkExperience>>();
        Assert.NotNull(sut);
        WorkExperience? updated = await sut.Create(_experience);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.Get(updated.Id);
        Assert.NotNull(updated);

        var edit = DateTime.UtcNow;
        updated.Until = new DateTime(edit.Ticks, DateTimeKind.Utc);
        updated = await sut.Update(updated);
        Assert.NotNull(updated);

        // Asserts.
        var existing = await sut.Get(updated.Id);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Until!.Value);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        var usersRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        _user = await usersRepo.Create(RepositoriesFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
        _experience = new WorkExperience()
        {
            JobTitle = $"Software developer",
            Company = "Company",
            Description = "Description",
            From = new DateTime(2018, 08, 01),
            Until = new DateTime(2026, 04, 21),
            ContactName = "Contact Name",
            ContactNumber = "Contact Number",
            UserId = _user.Id,
        };
    }
}