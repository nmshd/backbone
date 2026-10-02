using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backbone.BuildingBlocks.API.MinimalApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMinimalApiErrorHandling(this IServiceCollection services)
    {
        services.AddSingleton<HttpExceptionResponseFactory>();
        services.Configure<SwaggerGenOptions>(options => options.MapType<Base64QueryValue>(() => new OpenApiSchema { Type = JsonSchemaType.String, Format = "byte" }));
        services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

        return services;
    }
}
