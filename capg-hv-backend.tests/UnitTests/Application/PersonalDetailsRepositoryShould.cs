using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests.Application;

[Collection("Application services")]
public class PersonalDetailsRepositoryShould(ApplicationFixture fixture)
{
    private readonly ApplicationFixture _fixture = fixture;

    private PersonalDetails? _personalDetails;

    private User? _user;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_personalDetails);
        IRepository<PersonalDetails> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<PersonalDetails>>();
        Assert.NotNull(sut);

        // Creates
        PersonalDetails? created = await sut.Create(_personalDetails);
        Assert.NotNull(created);

        // Asserts.
        bool exists = await sut.Exists(created.Id);
        Assert.True(exists);

        List<PersonalDetails> all = await sut.List(_personalDetails.UserId);
        Assert.Single(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_personalDetails);
        IRepository<PersonalDetails> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<PersonalDetails>>();
        Assert.NotNull(sut);
        PersonalDetails? deleted = await sut.Create(_personalDetails);
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
        Assert.NotNull(_personalDetails);
        IRepository<PersonalDetails> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<PersonalDetails>>();
        Assert.NotNull(sut);
        PersonalDetails? updated = await sut.Create(_personalDetails);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.Get(updated.Id);
        Assert.NotNull(updated);

        string edit = "Edited";
        updated.Nationality = new string(edit);
        updated = await sut.Update(updated);
        Assert.NotNull(updated);

        // Asserts.
        PersonalDetails? existing = await sut.Get(updated.Id);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Nationality);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        IRepository<User> usersRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        _user = await usersRepo.Create(ApplicationFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
        _personalDetails = new PersonalDetails()
        {
            UserId = _user.Id,
            Nationality = "Colombian",
            TelephoneNumber = "123456789",
            BirthDate = new DateTime(1988, 7, 3).ToUniversalTime()
        };
    }
}