using capg_hv_backend.Endpoints.Abstractions;
using capg_hv_backend.Endpoints.Internal;
using Carter;

namespace capg_hv_backend.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        services.AddScoped<IFilesStorageOperations, FilesStorageOperations>();
        services.AddCarter();
        return services;
    }

    public static WebApplication UseEndpoints(this WebApplication app)
    {
        app.MapCarter();
        return app;
    }
}