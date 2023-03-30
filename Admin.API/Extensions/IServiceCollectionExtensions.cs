using Admin.API.Configuration;
using Microsoft.OpenApi.Models;

namespace Admin.API.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerWithCustomUi(this IServiceCollection services,
        AdminConfiguration.SwaggerUiConfiguration configuration)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        return services;
    }
}