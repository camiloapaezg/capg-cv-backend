using capg_hv_backend.Infrastructure.MessageBroker.Abstractions;
using capg_hv_backend.Infrastructure.MessageBroker.Internal;

namespace capg_hv_backend.Infrastructure.MessageBroker;

public static class DependencyInjection
{
    public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MessageBrokerOptions>(configuration.GetSection(nameof(MessageBrokerOptions)));
        services.AddTransient<IMessageBrokerClient, MessageBrokerClient>();

        return services;
    }
}