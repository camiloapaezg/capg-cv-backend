using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests;

[Collection("Repositories collection")]
public class FormalEducationRepositoryShould(RepositoriesFixture fixture)
{
    private readonly RepositoriesFixture _fixture = fixture;

    private FormalEducation? _formalEducation;

    private User? _user;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_formalEducation);
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<FormalEducation>>();
        Assert.NotNull(sut);

        // Creates
        var created = await sut.Create(_formalEducation);
        Assert.NotNull(created);

        // Asserts.
        var existing = await sut.Get(created.Id);
        Assert.NotNull(existing);

        var all = await sut.List(_formalEducation.UserId);
        Assert.Single(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_formalEducation);
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<FormalEducation>>();
        Assert.NotNull(sut);
        FormalEducation? deleted = await sut.Create(_formalEducation);
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
        Assert.NotNull(_formalEducation);
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<FormalEducation>>();
        Assert.NotNull(sut);
        FormalEducation? updated = await sut.Create(_formalEducation);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.Get(updated.Id);
        Assert.NotNull(updated);

        var edit = "Edited";
        updated.Description = new string(edit);
        updated = await sut.Update(updated);
        Assert.NotNull(updated);

        // Asserts.
        var existing = await sut.Get(updated.Id);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Description);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        var usersRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        _user = await usersRepo.Create(RepositoriesFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
        _formalEducation = new FormalEducation()
        {
            UserId = _user.Id,
            Degree = "Degree",
            School = "School",
            StartDate = "2006.06",
            EndDate = "2012.04"
        };
    }
}