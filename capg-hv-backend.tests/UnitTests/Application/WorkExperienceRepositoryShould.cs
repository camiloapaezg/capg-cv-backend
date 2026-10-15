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
        IListableRepository<WorkExperience> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<WorkExperience>>();
        Assert.NotNull(sut);

        // Creates
        WorkExperience? entity = await sut.CreateAsync(_experience, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts.
        entity = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

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
        IListableRepository<WorkExperience> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<WorkExperience>>();
        Assert.NotNull(sut);
        WorkExperience? entity = await sut.CreateAsync(_experience, TestContext.Current.CancellationToken);
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
        Assert.NotNull(_user);
        Assert.NotNull(_experience);
        IListableRepository<WorkExperience> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<WorkExperience>>();
        Assert.NotNull(sut);
        WorkExperience? updated = await sut.CreateAsync(_experience, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.ReadAsync(updated.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        DateTime edit = DateTime.UtcNow;
        updated.Until = new DateTime(edit.Ticks, DateTimeKind.Utc);
        updated = await sut.UpdateAsync(updated, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        // Asserts.
        WorkExperience? existing = await sut.ReadAsync(updated.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Until!.Value);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        IListableRepository<User> usersRepo = _fixture.TestHost.Services.GetRequiredService<IListableRepository<User>>();
        _user = await usersRepo.CreateAsync(ApplicationFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
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