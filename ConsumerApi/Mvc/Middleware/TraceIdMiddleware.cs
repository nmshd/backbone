namespace Backbone.ConsumerApi.Mvc.Middleware;

public class TraceIdMiddleware
{
    private const string RESPONSE_HEADER_TRACE_ID = "X-Trace-Id";

    private readonly RequestDelegate _next;

    public TraceIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var traceId = context.TraceIdentifier;
            context.Response.Headers[RESPONSE_HEADER_TRACE_ID] = traceId;
            return Task.CompletedTask;
        });

        return _next(context);
    }
}
