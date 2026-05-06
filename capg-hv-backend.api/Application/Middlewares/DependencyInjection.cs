using capg_hv_backend.Application.Middlewares.Internal;

namespace capg_hv_backend.Application.Middlewares;

public static class DependencyInjection
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceptionHandlingMiddleware>();
        builder.UseMiddleware<SecurityHeadersMiddleware>();

        return builder;
    }
}
