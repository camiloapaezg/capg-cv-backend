using System.Net;

namespace capg_hv_backend.Application.Middlewares.Internal;

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
            string message = "An unexpected error ocurred";
            string? details = _env.IsDevelopment() ? $"Exception '{ex.Source}' thrown: {ex.Message}. {ex.StackTrace}" : null;
            await httpContext.HandleExceptionAsync(HttpStatusCode.InternalServerError, message, details);
        }
    }
}