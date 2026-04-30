using capg_hv_backend.Application.Repositories;
using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Application.Repositories.Internal;
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

    public static string BucketName
    {
        get
        {
            return "test-bucket";
        }
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
        IHostBuilder builder = Host.CreateDefaultBuilder([])
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging()
                    .AddRepositories(context.Configuration);
                });

        return builder.Build();
    }

    public async Task DisposeAsync()
    {
        using IServiceScope scope = TestHost.Services.CreateScope();

        // Deletes the test bucket
        IFilesRepository filesRepository = scope.ServiceProvider.GetRequiredService<IFilesRepository>();
        await filesRepository.DeleteBucket(BucketName);
    }

    public async Task InitializeAsync()
    {
        using IServiceScope scope = TestHost.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}