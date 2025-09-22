using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

namespace Backbone.BuildingBlocks.API.Logging;

public static class LogHelper
{
    public static LogEventLevel GetLevel(HttpContext ctx, double _, Exception? ex)
    {
        return ex != null
            ? LogEventLevel.Error
            : ctx.Response.StatusCode > 499
                ? LogEventLevel.Error
                : IsHealthCheckEndpoint(ctx)
                    ? LogEventLevel.Verbose
                    : LogEventLevel.Information;
    }

    private static bool IsHealthCheckEndpoint(HttpContext ctx)
    {
        var endpoint = ctx.GetEndpoint();
        if (endpoint != null)
        {
            return string.Equals(
                endpoint.DisplayName,
                "Health checks",
                StringComparison.Ordinal);
        }

        return false;
    }

    public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;

        diagnosticContext.Set("Host", request.Host);
        diagnosticContext.Set("Protocol", request.Protocol);
        diagnosticContext.Set("Scheme", request.Scheme);

        if (request.QueryString.HasValue)
        {
            diagnosticContext.Set("QueryString", request.QueryString.Value);
        }

        if (httpContext.Response.ContentType != null)
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

        var endpoint = httpContext.GetEndpoint();

        if (endpoint?.DisplayName != null)
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
    }
}
