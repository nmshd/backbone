using Backbone.Tooling.Extensions;

namespace Backbone.ConsumerApi.Mvc.Middleware;

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
        var requestTime = DateTime.UtcNow.ToUniversalString();

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[RESPONSE_HEADER_REQUEST_TIME] = requestTime;
            context.Response.Headers[RESPONSE_HEADER_RESPONSE_TIME] = DateTime.UtcNow.ToUniversalString();
            return Task.CompletedTask;
        });

        return _next(context);
    }
}
