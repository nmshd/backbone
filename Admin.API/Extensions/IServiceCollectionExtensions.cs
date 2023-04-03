using Admin.API.Configuration;
using Microsoft.OpenApi.Models;

namespace Admin.API.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCustomSwaggerWithUi(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        return services;
    }
}