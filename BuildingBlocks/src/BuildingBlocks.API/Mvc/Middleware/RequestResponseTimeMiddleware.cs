using Backbone.Tooling;
using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore.Http;

namespace Backbone.BuildingBlocks.API.Mvc.Middleware;

public class RequestResponseTimeMiddleware
{
    private const string RESPONSE_HEADER_RESPONSE_TIME = "X-Response-Time";
    private const string RESPONSE_HEADER_REQUEST_TIME = "X-Request-Time";

    private readonly RequestDelegate _next;

    public RequestResponseTimeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        var requestTime = SystemTime.UtcNow.ToUniversalString();

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[RESPONSE_HEADER_REQUEST_TIME] = requestTime;
            context.Response.Headers[RESPONSE_HEADER_RESPONSE_TIME] = SystemTime.UtcNow.ToUniversalString();
            return Task.CompletedTask;
        });

        return _next(context);
    }
}
