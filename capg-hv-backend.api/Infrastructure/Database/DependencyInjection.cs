using capg_hv_backend.Infrastructure.Database.Internal;
using Microsoft.EntityFrameworkCore;

namespace capg_hv_backend.Infrastructure.Database;

public static class DependencyInjection
{
    private static readonly int _maxRetryCount = 3;

    private static readonly TimeSpan _maxRetryDelay = TimeSpan.FromSeconds(10);

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // EF configuration
        services.Configure<DatabaseOptions>(configuration.GetSection(nameof(DatabaseOptions)));
        DatabaseOptions? options = configuration.GetRequiredSection(nameof(DatabaseOptions)).Get<DatabaseOptions>();
        if (options is not null)
        {
            services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(options.ConnectionString, options =>
            {
                options.EnableRetryOnFailure(
                    maxRetryCount: _maxRetryCount,
                    maxRetryDelay: _maxRetryDelay,
                    errorCodesToAdd: null);
            }));
        }

        return services;
    }

    public static IHost InitializeDatabase(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();

        return host;
    }
}