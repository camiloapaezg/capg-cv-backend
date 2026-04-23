using capg_hv_backend.Application.Persistence;
using capg_hv_backend.Application.Persistence.Internal;
using capg_hv_backend.Application.Repositories;
using capg_hv_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace capg_hv_backend.tests.Fixtures;

public class RepositoriesFixture : IAsyncLifetime
{
    public RepositoriesFixture()
    {
        TestHost = CreateHost();
    }

    public static User DefaultUser
    {
        get
        {
            return new User()
            {
                FirstName = "Camilo",
                LastName = "Paez",
                EmailAddress = "camiloapaezg@gmail.com"
            };
        }
    }

    public IHost TestHost { get; init; }

    public static IHost CreateHost()
    {
        var builder = Host.CreateDefaultBuilder([])
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging()
                    .AddPersistence(context.Configuration)
                    .AddRepositories();
                });

        return builder.Build();
    }

    public virtual async Task DisposeAsync()
    {
        // Deletes the database.
        using var scope = TestHost.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
    }

    public virtual async Task InitializeAsync()
    {
        using var scope = TestHost.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}