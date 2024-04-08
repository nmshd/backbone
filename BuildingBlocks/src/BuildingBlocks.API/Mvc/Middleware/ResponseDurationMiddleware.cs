using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Backbone.BuildingBlocks.API.Mvc.Middleware;

public class ResponseDurationMiddleware
{
    private const string RESPONSE_HEADER_RESPONSE_DURATION = "X-Response-Duration-ms";

    private readonly RequestDelegate _next;

    public ResponseDurationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        var watch = new Stopwatch();
        watch.Start();
        context.Response.OnStarting(() =>
        {
            watch.Stop();
            var responseTimeForCompleteRequest = watch.ElapsedMilliseconds;
            context.Response.Headers[RESPONSE_HEADER_RESPONSE_DURATION] = responseTimeForCompleteRequest.ToString();
            return Task.CompletedTask;
        });
        return _next(context);
    }
}
