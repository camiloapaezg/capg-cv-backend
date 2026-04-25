using capg_hv_backend.InterfaceAdapters.Entities;
using System.Net;

namespace capg_hv_backend.InterfaceAdapters;

public static class ExtensionMethods
{
    public static async Task HandleExceptionAsync(this HttpContext context, HttpStatusCode statusCode, string? message = null, string? details = null)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        var response = new HttpErrorDetails((int)statusCode, message, details);
        await context.Response.WriteAsJsonAsync(response);
    }
}