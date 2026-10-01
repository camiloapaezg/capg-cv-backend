using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests.Application;

[Collection("Application services")]
public class WorkExperienceRepositoryShould(ApplicationFixture fixture)
{
    private readonly ApplicationFixture _fixture = fixture;

    private WorkExperience? _experience;

    private User? _user;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_experience);
        IRepository<WorkExperience> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<WorkExperience>>();
        Assert.NotNull(sut);

        // Creates
        WorkExperience? created = await sut.Create(_experience, TestContext.Current.CancellationToken);
        Assert.NotNull(created);

        // Asserts.
        bool exists = await sut.Exists(created.Id, TestContext.Current.CancellationToken);
        Assert.True(exists);

        List<WorkExperience> all = await sut.List(_experience.UserId, TestContext.Current.CancellationToken);
        Assert.Single(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_experience);
        IRepository<WorkExperience> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<WorkExperience>>();
        Assert.NotNull(sut);
        WorkExperience? deleted = await sut.Create(_experience, TestContext.Current.CancellationToken);
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
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_experience);
        IRepository<WorkExperience> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<WorkExperience>>();
        Assert.NotNull(sut);
        WorkExperience? updated = await sut.Create(_experience, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.Get(updated.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        DateTime edit = DateTime.UtcNow;
        updated.Until = new DateTime(edit.Ticks, DateTimeKind.Utc);
        updated = await sut.Update(updated, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        // Asserts.
        WorkExperience? existing = await sut.Get(updated.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Until!.Value);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        IRepository<User> usersRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        _user = await usersRepo.Create(ApplicationFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
        _experience = new WorkExperience()
        {
            JobTitle = $"Software developer",
            Company = "Company",
            Location = "Germany",
            Description = "Description",
            From = new DateTime(2018, 08, 01),
            Until = new DateTime(2026, 04, 21),
            ContactName = "Contact Name",
            ContactNumber = "Contact Number",
            UserId = _user.Id,
        };
    }
}