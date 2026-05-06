using capg_hv_backend.Application.FilesValidator.Abstractions;

namespace capg_hv_backend.Application.FilesValidator;

public static class DependencyInjection
{
    public static IServiceCollection AddFilesValidator(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileValidationOptions>(configuration.GetSection(nameof(FileValidationOptions)));
        services.AddTransient<IFilesValidator, Internal.FilesValidator>();

        return services;
    }
}