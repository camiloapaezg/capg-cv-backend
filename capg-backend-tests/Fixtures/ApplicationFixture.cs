using capg_hv_backend.Application.FilesValidator;
using capg_hv_backend.Application.Repositories;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.Infrastructure.Database;
using capg_hv_backend.Infrastructure.FileStorage;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace capg_hv_backend.tests.Fixtures;

public class ApplicationFixture : IAsyncLifetime
{
    private readonly IContainer _database;

    private readonly IContainer _fileStorage;

    public ApplicationFixture()
    {
        // Creates infrastructure
        INetwork network = CreateNetwork();
        _database = BuildDatabase(network);
        _fileStorage = BuildFileStorage(network);

        // Creates host with services under test.
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

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _database.DisposeAsync();
        await _fileStorage.DisposeAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await _database.StartAsync().ConfigureAwait(false);
        await _fileStorage.StartAsync().ConfigureAwait(false);
        TestHost.InitializeDatabase();
    }

    private static INetwork CreateNetwork() => new NetworkBuilder()
            .WithName("app-fixture-net")
            .WithCleanUp(true)
            .Build();

    private static IHost CreateHost()
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

    private static IContainer BuildDatabase(INetwork network) => new ContainerBuilder("postgres:latest")
        .WithName("database")
        .WithNetwork(network)
        .WithPortBinding(5432, false)
        .WithEnvironment("POSTGRES_USER", "camiloapaezg")
        .WithEnvironment("POSTGRES_PASSWORD", "c4m1l0Ap43zG")
        .WithEnvironment("POSTGRES_DB", "capg-hv-db")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("database system is ready to accept connection"))
        .Build();

    private static IContainer BuildFileStorage(INetwork network) => new ContainerBuilder("chrislusf/seaweedfs:4.21")
        .WithName("blob")
        .WithNetwork(network)
        .WithPortBinding(8333, false)
        .WithBindMount(Path.Combine(Directory.GetCurrentDirectory(), "Resources/Blob/s3.json"), "/etc/seaweedfs/s3.json", AccessMode.ReadOnly)
        .WithBindMount(Path.Combine(Directory.GetCurrentDirectory(), "Resources/Blob/filer.toml"), "/etc/seaweedfs/filer.toml", AccessMode.ReadOnly)
        .WithCommand("server -s3 -dir=/data -s3.config=/etc/seaweedfs/s3.json")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Start Seaweed S3 API Server"))
        .Build();
}