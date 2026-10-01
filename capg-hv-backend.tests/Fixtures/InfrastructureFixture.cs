using capg_hv_backend.Infrastructure.FilesScanner;
using capg_hv_backend.Infrastructure.MessageBroker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace capg_hv_backend.tests.Fixtures;

public class InfrastructureFixture : IAsyncLifetime
{
    public InfrastructureFixture()
    {
        TestHost = CreateHost();
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
                    .AddMessageBroker(context.Configuration)
                    .AddFilesScanner(context.Configuration);
                });

        return builder.Build();
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }
}