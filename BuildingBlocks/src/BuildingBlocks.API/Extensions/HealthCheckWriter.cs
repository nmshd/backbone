using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Backbone.BuildingBlocks.API.Extensions;

public static class HealthCheckWriter
{
    public static Task WriteResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "text/plain; charset=utf-8";

        return context.Response.WriteAsync(
            healthReport.Status.ToString()
        );
    }
}
