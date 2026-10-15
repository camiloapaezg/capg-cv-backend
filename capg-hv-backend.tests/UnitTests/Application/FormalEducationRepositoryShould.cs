using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests.Application;

[Collection("Application services")]
public class FormalEducationRepositoryShould(ApplicationFixture fixture)
{
    private readonly ApplicationFixture _fixture = fixture;

    private FormalEducation? _formalEducation;

    private User? _user;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_formalEducation);
        IListableRepository<FormalEducation> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<FormalEducation>>();
        Assert.NotNull(sut);

        // Creates
        FormalEducation? entity = await sut.CreateAsync(_formalEducation, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts.
        entity = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        List<FormalEducation> all = await sut.List(_formalEducation.UserId, TestContext.Current.CancellationToken);
        Assert.Single(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_formalEducation);
        IListableRepository<FormalEducation> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<FormalEducation>>();
        Assert.NotNull(sut);
        FormalEducation? entity = await sut.CreateAsync(_formalEducation, TestContext.Current.CancellationToken);
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
        Assert.NotNull(_formalEducation);
        IListableRepository<FormalEducation> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<FormalEducation>>();
        Assert.NotNull(sut);
        FormalEducation? updated = await sut.CreateAsync(_formalEducation, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.ReadAsync(updated.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        string edit = "Edited";
        updated.Description = edit;
        updated = await sut.UpdateAsync(updated, TestContext.Current.CancellationToken);
        Assert.NotNull(updated);

        // Asserts.
        FormalEducation? existing = await sut.ReadAsync(updated.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Description);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        IListableRepository<User> usersRepo = _fixture.TestHost.Services.GetRequiredService<IListableRepository<User>>();
        _user = await usersRepo.CreateAsync(ApplicationFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
        _formalEducation = new FormalEducation()
        {
            UserId = _user.Id,
            Degree = "Degree",
            Institution = "School",
            StartDate = "2006.06",
            EndDate = "2012.04"
        };
    }
}