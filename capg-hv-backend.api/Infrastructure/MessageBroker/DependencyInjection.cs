using capg_hv_backend.Infrastructure.MessageBroker.Abstractions;

namespace capg_hv_backend.Infrastructure.MessageBroker;

public static class DependencyInjection
{
    public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MessageBrokerOptions>(configuration.GetSection(nameof(MessageBrokerOptions)));
        services.AddTransient<IMessageBroker, Internal.MessageBroker>();

        return services;
    }
}
