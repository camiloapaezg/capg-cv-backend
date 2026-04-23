using capg_hv_backend.Application.Persistence.Internal;
using Microsoft.EntityFrameworkCore;

namespace capg_hv_backend.Application.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PersistenceOptions>(configuration.GetSection(nameof(PersistenceOptions)));
        var options = configuration.GetRequiredSection(nameof(PersistenceOptions)).Get<PersistenceOptions>();
        if (options is not null)
        {
            services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(options.ConnectionString));
        }

        return services;
    }

    public static IHost UsePersistence(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();

        return host;
    }
}