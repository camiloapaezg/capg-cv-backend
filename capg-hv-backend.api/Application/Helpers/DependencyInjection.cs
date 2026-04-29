using capg_hv_backend.Application.Helpers.Abstractions;
using capg_hv_backend.Application.Helpers.Internal;

namespace capg_hv_backend.Application.Helpers;

public static class DependencyInjection
{
    public static IServiceCollection AddHelpers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.Configure<AntivirusServiceOptions>(configuration.GetSection(nameof(AntivirusServiceOptions)));
        services.Configure<FileValidationOptions>(configuration.GetSection(nameof(FileValidationOptions)));
        services.AddSingleton<IFilesValidatorHelper, FilesValidatorHelper>();
        services.AddSingleton<IAntivirusServiceHelper, AntivirusServiceHelper>();

        return services;
    }
}
