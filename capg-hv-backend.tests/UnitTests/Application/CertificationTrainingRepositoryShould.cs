using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests.Application;

[Collection("Application services")]
public class CertificationTrainingRepositoryShould(ApplicationFixture fixture)
{
    private readonly ApplicationFixture _fixture = fixture;

    private CertificationTraining? _certification;

    private User? _user;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_certification);
        IListableRepository<CertificationTraining> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<CertificationTraining>>();
        Assert.NotNull(sut);

        // Creates
        CertificationTraining? entity = await sut.CreateAsync(_certification, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts.
        entity = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        List<CertificationTraining> all = await sut.List(_certification.UserId, TestContext.Current.CancellationToken);
        Assert.Single(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_certification);
        IListableRepository<CertificationTraining> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<CertificationTraining>>();
        Assert.NotNull(sut);
        CertificationTraining? entity = await sut.CreateAsync(_certification, TestContext.Current.CancellationToken);
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
        Assert.NotNull(_certification);
        IListableRepository<CertificationTraining> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<CertificationTraining>>();
        Assert.NotNull(sut);
        CertificationTraining? entity = await sut.CreateAsync(_certification, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Updates.
        entity = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        string edit = "Edited";
        entity.Institution = edit;
        entity = await sut.UpdateAsync(entity, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts.
        CertificationTraining? existing = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Institution);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        IListableRepository<User> usersRepo = _fixture.TestHost.Services.GetRequiredService<IListableRepository<User>>();
        _user = await usersRepo.CreateAsync(ApplicationFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
        _certification = new CertificationTraining()
        {
            Title = $"Certification Title",
            Institution = "Certification Institution",
            FinishedAt = "2012.04",
            UserId = _user.Id,
        };
    }
}