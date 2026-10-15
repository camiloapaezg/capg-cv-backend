using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests.Application;

[Collection("Application services")]
public class PublicationRepositoryShould(ApplicationFixture fixture)
{
    private readonly ApplicationFixture _fixture = fixture;

    private Publication? _publication;

    private User? _user;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_publication);
        IListableRepository<Publication> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<Publication>>();
        Assert.NotNull(sut);

        // Creates
        Publication? entity = await sut.CreateAsync(_publication, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts.
        entity = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        List<Publication> all = await sut.List(_publication.UserId, TestContext.Current.CancellationToken);
        Assert.Single(all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        await CreateTestEntities();
        Assert.NotNull(_user);
        Assert.NotNull(_publication);
        IListableRepository<Publication> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<Publication>>();
        Assert.NotNull(sut);
        Publication? entity = await sut.CreateAsync(_publication, TestContext.Current.CancellationToken);
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
        Assert.NotNull(_publication);
        IListableRepository<Publication> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<Publication>>();
        Assert.NotNull(sut);
        Publication? entity = await sut.CreateAsync(_publication, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Updates.
        entity = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        string edit = "Edited";
        entity.Location = edit;
        entity = await sut.UpdateAsync(entity, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts.
        Publication? existing = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.Location);
    }

    private async Task CreateTestEntities()
    {
        // Initializes entities
        IListableRepository<User> usersRepo = _fixture.TestHost.Services.GetRequiredService<IListableRepository<User>>();
        _user = await usersRepo.CreateAsync(ApplicationFixture.DefaultUser) ?? throw new ArgumentNullException(nameof(_user));
        _publication = new Publication()
        {
            Title = "Publication Title",
            Type = "Type",
            Location = "Germany",
            Doi = "123456",
            PublishedAt = "2012.04",
            UserId = _user.Id,
        };
    }
}