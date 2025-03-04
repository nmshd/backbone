using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
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

        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = CustomLogContext.GenerateCorrelationId();
        }
        else
        {
            correlationId = correlationId.Trim().ReplaceLineEndings("")[..100];
        }

        context.Response.Headers["X-Correlation-ID"] = correlationId;

        using (CustomLogContext.SetCorrelationId(correlationId))
        {
            await _next(context);
        }
    }
}
