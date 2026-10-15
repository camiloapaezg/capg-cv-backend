using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests.Application;

[Collection("Application services")]
public class GeneralDetailsRepositoryShould(ApplicationFixture fixture)
{
    private readonly ApplicationFixture _fixture = fixture;

    private GeneralDetails? _generalDetails;

    private User? _user;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_generalDetails);
        ICrudRepository<GeneralDetails> sut = _fixture.TestHost.Services.GetRequiredService<ICrudRepository<GeneralDetails>>();
        Assert.NotNull(sut);

        // Creates
        GeneralDetails? entity = await sut.CreateAsync(_generalDetails, TestContext.Current.CancellationToken);
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
        Assert.NotNull(_generalDetails);
        ICrudRepository<GeneralDetails> sut = _fixture.TestHost.Services.GetRequiredService<ICrudRepository<GeneralDetails>>();
        Assert.NotNull(sut);
        GeneralDetails? entity = await sut.CreateAsync(_generalDetails, TestContext.Current.CancellationToken);
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
        Assert.NotNull(_generalDetails);
        ICrudRepository<GeneralDetails> sut = _fixture.TestHost.Services.GetRequiredService<ICrudRepository<GeneralDetails>>();
        Assert.NotNull(sut);
        GeneralDetails? entity = await sut.CreateAsync(_generalDetails, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Updates.
        entity = await sut.ReadAsync(_user.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        string edit = "Edited";
        entity.Description = new string(edit);
        entity = await sut.UpdateAsync(entity, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts.
        GeneralDetails? existing = await sut.ReadAsync(entity.UserId, TestContext.Current.CancellationToken);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Description);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        IListableRepository<User> usersRepo = _fixture.TestHost.Services.GetRequiredService<IListableRepository<User>>();
        _user = await usersRepo.CreateAsync(ApplicationFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
        _generalDetails = new GeneralDetails()
        {
            UserId = _user.Id,
            Title = "Title"
        };
    }
}