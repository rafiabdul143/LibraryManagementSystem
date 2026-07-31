using System.Text.Json;

namespace LibraryManagement.Web.Middleware;

/// <summary>
/// Outermost middleware in the pipeline (registered first in Program.cs).
/// Catches any exception not already handled further down the pipeline
/// (e.g. by UseExceptionHandler in production), logs it via Serilog with
/// full context, and returns either a JSON error payload (for AJAX/API
/// callers) or redirects the browser to the friendly /Home/Error page.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception processing {Method} {Path} (TraceId: {TraceId})",
                context.Request.Method, context.Request.Path, context.TraceIdentifier);

            await HandleExceptionAsync(context);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context)
    {
        // If the response has already started streaming, headers/status code
        // can no longer be changed - nothing safe left to do but rethrow so
        // the failure isn't silently swallowed.
        if (context.Response.HasStarted)
        {
            throw new InvalidOperationException(
                "An exception occurred after the response had already started; see the inner/logged exception for details.");
        }

        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var wantsJson =
            context.Request.Headers.Accept.ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        if (wantsJson)
        {
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(new
            {
                error = "An unexpected error occurred. Please try again later.",
                requestId = context.TraceIdentifier
            });
            await context.Response.WriteAsync(payload);
        }
        else
        {
            context.Response.Redirect($"/Home/Error?requestId={context.TraceIdentifier}");
        }
    }
}