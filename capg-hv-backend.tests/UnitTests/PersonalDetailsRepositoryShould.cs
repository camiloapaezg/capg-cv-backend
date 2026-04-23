using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests;

[Collection("Repositories collection")]
public class PersonalDetailsRepositoryShould(RepositoriesFixture fixture)
{
    private readonly RepositoriesFixture _fixture = fixture;

    private PersonalDetails? _personalDetails;
    private User? _user;
    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_personalDetails);
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<PersonalDetails>>();
        Assert.NotNull(sut);

        // Creates
        var created = await sut.Create(_personalDetails);
        Assert.NotNull(created);

        // Asserts.
        var existing = await sut.Get(created.Id);
        Assert.NotNull(existing);

        var all = await sut.List(_personalDetails.UserId);
        Assert.Single(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_personalDetails);
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<PersonalDetails>>();
        Assert.NotNull(sut);
        PersonalDetails? deleted = await sut.Create(_personalDetails);
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
        Assert.NotNull(_personalDetails);
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<PersonalDetails>>();
        Assert.NotNull(sut);
        PersonalDetails? updated = await sut.Create(_personalDetails);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.Get(updated.Id);
        Assert.NotNull(updated);

        var edit = "Edited";
        updated.Nationality = new string(edit);
        updated = await sut.Update(updated);
        Assert.NotNull(updated);

        // Asserts.
        var existing = await sut.Get(updated.Id);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Nationality);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        var usersRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        _user = await usersRepo.Create(RepositoriesFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
        _personalDetails = new PersonalDetails()
        {
            UserId = _user.Id,
            Nationality = "Colombian",
            TelephoneNumber = "123456789",
            BirthDate = new DateTime(1988, 7, 3).ToUniversalTime()
        };
    }
}