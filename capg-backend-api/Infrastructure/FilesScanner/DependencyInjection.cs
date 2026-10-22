using capg_hv_backend.Infrastructure.FilesScanner.Abstractions;
using capg_hv_backend.Infrastructure.FilesScanner.Internal;

namespace capg_hv_backend.Infrastructure.FilesScanner;

public static class DependencyInjection
{
    public static IServiceCollection AddFilesScanner(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FilesScannerOptions>(configuration.GetSection(nameof(FilesScannerOptions)));
        services.AddTransient<IFilesScannerClient, FilesScannerClient>();

        return services;
    }
}