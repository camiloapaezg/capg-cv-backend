using capg_hv_backend.Infrastructure.Database;
using capg_hv_backend.Infrastructure.FilesScanner;
using capg_hv_backend.Infrastructure.FileStorage;
using capg_hv_backend.Infrastructure.IdentityProvider;
using capg_hv_backend.Infrastructure.MessageBroker;

namespace capg_hv_backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration)
            .AddFileStorage(configuration)
            .AddFilesScanner(configuration)
            .AddMessageBroker(configuration)
            .AddIdentityProvider(configuration);

        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseIdentityProvider();
        app.InitializeDatabase();

        return app;
    }
}