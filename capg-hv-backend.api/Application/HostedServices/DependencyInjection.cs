using capg_hv_backend.Application.HostedServices.Internal;

namespace capg_hv_backend.Application.HostedServices;

public static class DependencyInjection
{
    public static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<FilesScannerService>();
        services.AddHostedService<FilesDeleteService>();

        return services;
    }
}
