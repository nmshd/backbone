using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.BuildingBlocks.API.MinimalApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMinimalApiErrorHandling(this IServiceCollection services)
    {
        services.AddSingleton<HttpExceptionResponseFactory>();
        services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

        return services;
    }
}
