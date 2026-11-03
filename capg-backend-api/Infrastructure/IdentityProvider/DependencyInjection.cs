using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace capg_hv_backend.Infrastructure.IdentityProvider;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityProvider(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IdentityProviderOptions>(configuration.GetSection(nameof(IdentityProviderOptions)));

        IdentityProviderOptions? iamOptions = configuration.GetSection(nameof(IdentityProviderOptions)).Get<IdentityProviderOptions>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = iamOptions?.Endpoint;
                options.Audience = iamOptions?.ClientId;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = iamOptions?.Endpoint
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static WebApplication UseIdentityProvider(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}