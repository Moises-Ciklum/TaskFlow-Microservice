using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CoupleCalendar.API.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Default to 500 Internal Server Error
        int statusCode = StatusCodes.Status500InternalServerError;
        string title = "Server Error";

        // If it's our Business Rule exception, switch to 400 Bad Request
        if (exception is InvalidOperationException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            title = "Business Rule Violation";
        }

        // Format the response using the standard ProblemDetails structure
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Return true to tell .NET: "I successfully handled this error, don't crash the app"
        return true;
    }
}