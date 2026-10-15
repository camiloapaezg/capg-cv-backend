using capg_hv_backend.Application.FilesValidator;
using capg_hv_backend.Application.Repositories;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Infrastructure.Database;
using capg_hv_backend.Infrastructure.Database.Internal;
using capg_hv_backend.Infrastructure.FileStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace capg_hv_backend.tests.Fixtures;

public class ApplicationFixture : IAsyncLifetime
{
    public ApplicationFixture()
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
        IHostBuilder builder = Host.CreateDefaultBuilder([])
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging()
                    .AddDatabase(context.Configuration)
                    .AddFileStorage(context.Configuration)
                    .AddRepositories(context.Configuration)
                    .AddFilesValidator(context.Configuration);
                });

        return builder.Build();
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    public async ValueTask InitializeAsync()
    {
        using IServiceScope scope = TestHost.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}