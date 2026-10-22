using capg_hv_backend.Infrastructure.FileStorage.Abstractions;
using capg_hv_backend.Infrastructure.FileStorage.Internal;

namespace capg_hv_backend.Infrastructure.FileStorage;

public static class DependencyInjection
{
    public static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileStorageOptions>(configuration.GetSection(nameof(FileStorageOptions)));
        services.AddScoped<IFileStorageClient, FileStorageClient>();

        return services;
    }
}