using capg_hv_backend.Infrastructure.FilesScanner.Abstractions;

namespace capg_hv_backend.Infrastructure.FilesScanner;

public static class DependencyInjection
{
    public static IServiceCollection AddFilesScanner(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FilesScannerOptions>(configuration.GetSection(nameof(FilesScannerOptions)));
        services.AddTransient<IFilesScanner, Internal.FilesScanner>();

        return services;
    }
}
