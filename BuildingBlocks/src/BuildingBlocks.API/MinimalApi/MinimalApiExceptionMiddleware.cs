using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.API.MinimalApi;

public sealed class MinimalApiExceptionMiddleware(RequestDelegate next, ILogger<MinimalApiExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, HttpExceptionResponseFactory responseFactory)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception) when (context.GetEndpoint()?.Metadata.GetMetadata<MinimalApiEndpointMetadata>() != null && !context.Response.HasStarted)
        {
            logger.LogInformation(exception, "Handled exception in Minimal API endpoint.");
            var response = responseFactory.Create(exception);
            context.Response.StatusCode = (int)response.StatusCode;
            await context.Response.WriteAsJsonAsync(HttpResponseEnvelope.CreateError(response.Error));
        }
    }
}
