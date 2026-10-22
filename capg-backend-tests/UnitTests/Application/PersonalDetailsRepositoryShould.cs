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
        ICrudRepository<PersonalDetails> sut = _fixture.TestHost.Services.GetRequiredService<ICrudRepository<PersonalDetails>>();
        Assert.NotNull(sut);

        // Creates
        PersonalDetails? entity = await sut.CreateAsync(_personalDetails, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts.
        entity = await sut.ReadAsync(_user.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_personalDetails);
        ICrudRepository<PersonalDetails> sut = _fixture.TestHost.Services.GetRequiredService<ICrudRepository<PersonalDetails>>();
        Assert.NotNull(sut);
        PersonalDetails? entity = await sut.CreateAsync(_personalDetails, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);
        entity = await sut.ReadAsync(_user.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Deletes
        entity = await sut.DeleteAsync(_user.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts
        entity = await sut.ReadAsync(_user.Id, TestContext.Current.CancellationToken);
        Assert.Null(entity);
    }

    [Fact]
    public async Task UpdateEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_personalDetails);
        ICrudRepository<PersonalDetails> sut = _fixture.TestHost.Services.GetRequiredService<ICrudRepository<PersonalDetails>>();
        Assert.NotNull(sut);
        PersonalDetails? entity = await sut.CreateAsync(_personalDetails, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Updates.
        entity = await sut.ReadAsync(_user.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        string edit = "Edited";
        entity.Nationality = edit;
        entity = await sut.UpdateAsync(entity, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts.
        PersonalDetails? existing = await sut.ReadAsync(_user.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Nationality);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        IListableRepository<User> usersRepo = _fixture.TestHost.Services.GetRequiredService<IListableRepository<User>>();
        _user = await usersRepo.CreateAsync(ApplicationFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
        _personalDetails = new PersonalDetails()
        {
            UserId = _user.Id,
            Nationality = "Colombian",
            TelephoneNumber = "123456789",
            Location = "Augsburg",
            BirthDate = new DateTime(1988, 7, 3).ToUniversalTime()
        };
    }
}