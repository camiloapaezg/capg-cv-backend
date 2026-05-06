using capg_hv_backend.Infrastructure.Persistence.Internal;
using Microsoft.EntityFrameworkCore;

namespace capg_hv_backend.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // EF configuration
        services.Configure<DatabaseOptions>(configuration.GetSection(nameof(DatabaseOptions)));
        DatabaseOptions? options = configuration.GetRequiredSection(nameof(DatabaseOptions)).Get<DatabaseOptions>();
        if (options is not null)
        {
            services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(options.ConnectionString));
        }

        return services;
    }

    public static IHost UsePersistence(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();

        return host;
    }
}