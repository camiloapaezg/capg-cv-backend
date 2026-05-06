using capg_hv_backend.Infrastructure.FilesScanner;
using capg_hv_backend.Infrastructure.MessageBroker;
using capg_hv_backend.Infrastructure.Persistence;

namespace capg_hv_backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration)
            .AddFilesScanner(configuration)
            .AddMessageBroker(configuration);

        return services;
    }

    public static IHost UseInfrastructure(this IHost host)
    {
        host.UsePersistence();

        return host;
    }
}