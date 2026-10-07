using capg_hv_backend.Application.Channels.Abstractions;
using capg_hv_backend.Application.Channels.Entities;

namespace capg_hv_backend.Application.Channels;

public static class DependencyInjection
{
    public static IServiceCollection AddChannels(this IServiceCollection services)
    {
        services.AddSingleton<IChannel<FileDeleteRequestDto>, FilesDeleteChannel>();

        return services;
    }
}
