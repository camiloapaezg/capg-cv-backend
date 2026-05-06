using capg_hv_backend.Application.FilesValidator;
using capg_hv_backend.Application.HostedServices;
using capg_hv_backend.Application.Repositories;
using capg_hv_backend.Application.Validators;

namespace capg_hv_backend.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositories(configuration)
            .AddValidators()
            .AddFilesValidator(configuration)
            .AddHostedServices();

        return services;
    }
}