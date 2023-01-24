using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Enmeshed.BuildingBlocks.API.Mvc.Middleware
{
    public class RequestIdMiddleware
    {
        private const string RESPONSE_HEADER_TRACE_ID = "X-Trace-Id";

        private readonly RequestDelegate _next;

        public RequestIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
                context.Response.Headers[RESPONSE_HEADER_TRACE_ID] = traceId;
                return Task.CompletedTask;
            });

            return _next(context);
        }
    }
}