using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore.Http;

namespace Backbone.BuildingBlocks.API.Mvc.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();

        correlationId = string.IsNullOrEmpty(correlationId)
            ? CustomLogContext.GenerateCorrelationId()
            : correlationId.Trim().ReplaceLineEndings("").TruncateToXChars(100);

        context.Response.Headers["X-Correlation-ID"] = correlationId;

        using (CustomLogContext.SetCorrelationId(correlationId))
        {
            await _next(context);
        }
    }
}
