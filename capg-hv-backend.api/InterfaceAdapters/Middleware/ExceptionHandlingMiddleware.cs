using System.Net;

namespace capg_hv_backend.InterfaceAdapters.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env)
{
    private readonly IWebHostEnvironment _env = env;

    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ArgumentNullException)
        {
            await httpContext.HandleExceptionAsync(HttpStatusCode.BadRequest, "Missing required parameter.");
        }
        catch (UnauthorizedAccessException)
        {
            await httpContext.HandleExceptionAsync(HttpStatusCode.Unauthorized, "Access denied.");
        }
        catch (Exception ex)
        {
            var message = "An unexpected error ocurred";
            var details = _env.IsDevelopment() ? $"Exception '{ex.Source}' thrown: {ex.Message}. {ex.StackTrace}" : null;
            await httpContext.HandleExceptionAsync(HttpStatusCode.InternalServerError, message, details);
        }
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}