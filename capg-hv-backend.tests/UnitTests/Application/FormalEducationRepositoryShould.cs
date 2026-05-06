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
        IRepository<FormalEducation> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<FormalEducation>>();
        Assert.NotNull(sut);

        // Creates
        FormalEducation? created = await sut.Create(_formalEducation);
        Assert.NotNull(created);

        // Asserts.
        bool exists = await sut.Exists(created.Id);
        Assert.True(exists);

        List<FormalEducation> all = await sut.List(_formalEducation.UserId);
        Assert.Single(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_formalEducation);
        IRepository<FormalEducation> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<FormalEducation>>();
        Assert.NotNull(sut);
        FormalEducation? deleted = await sut.Create(_formalEducation);
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
        Assert.NotNull(_formalEducation);
        IRepository<FormalEducation> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<FormalEducation>>();
        Assert.NotNull(sut);
        FormalEducation? updated = await sut.Create(_formalEducation);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.Get(updated.Id);
        Assert.NotNull(updated);

        string edit = "Edited";
        updated.Description = new string(edit);
        updated = await sut.Update(updated);
        Assert.NotNull(updated);

        // Asserts.
        FormalEducation? existing = await sut.Get(updated.Id);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Description);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        IRepository<User> usersRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        _user = await usersRepo.Create(ApplicationFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
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