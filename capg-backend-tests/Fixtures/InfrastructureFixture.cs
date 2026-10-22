using capg_hv_backend.Application.Resiliency;
using capg_hv_backend.Infrastructure.FilesScanner;
using capg_hv_backend.Infrastructure.MessageBroker;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace capg_hv_backend.tests.Fixtures;

public class InfrastructureFixture : IAsyncLifetime
{
    private readonly IContainer _fileScanner;

    private readonly IContainer _messageBroker;

    public InfrastructureFixture()
    {
        // Creates infrastructure
        INetwork network = CreateNetwork();
        _fileScanner = BuildFileScanner(network);
        _messageBroker = BuildMessageBroker(network);

        // Creates host with services under test.
        TestHost = CreateHost();
    }

    public IHost TestHost { get; init; }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _fileScanner.DisposeAsync();
        await _messageBroker.DisposeAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await _fileScanner.StartAsync();
        await _messageBroker.StartAsync();
    }

    private static IContainer BuildFileScanner(INetwork network) => new ContainerBuilder("clamav/clamav:1.5.2_base")
        .WithName("file-scanner")
        .WithNetwork(network)
        .WithPortBinding(3310, false)
        .WithBindMount(Path.Combine(Directory.GetCurrentDirectory(), "Resources/Antivirus/clamd.conf"), "/etc/clamav/clamd.conf", AccessMode.ReadOnly)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("clamd started"))
        .Build();

    private static IContainer BuildMessageBroker(INetwork network) => new ContainerBuilder("rabbitmq:4.3.0-management")
        .WithName("message-broker")
        .WithNetwork(network)
        .WithPortBinding(5672, false)
        .WithEnvironment("RABBITMQ_DEFAULT_USER", "camiloapaezg")
        .WithEnvironment("RABBITMQ_DEFAULT_PASS", "c4m1l0Ap43zG")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Server startup complete"))
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
                    .AddResiliency()
                    .AddMessageBroker(context.Configuration)
                    .AddFilesScanner(context.Configuration);
                });

        return builder.Build();
    }

    private static INetwork CreateNetwork() => new NetworkBuilder()
                                    .WithName("infra-fixture-net")
            .WithCleanUp(true)
            .Build();
}