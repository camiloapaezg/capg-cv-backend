using capg_hv_backend.Application.Middlewares.Entities;
using System.Net;

namespace capg_hv_backend.Application.Middlewares;

public static class ExtensionMethods
{
    public static async Task HandleExceptionAsync(this HttpContext context, HttpStatusCode statusCode, string? message = null, string? details = null)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        HttpErrorDetails response = new((int)statusCode, message, details);
        await context.Response.WriteAsJsonAsync(response);
    }
}