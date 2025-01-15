using Backbone.AdminApi.Configuration;
using Backbone.Modules.Tokens.Application.Extensions;

namespace Backbone.AdminApi.Extensions;

public static class TokensServiceCollectionExtensions
{
    public static IServiceCollection AddTokens(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication(configuration.GetSection("Application"));

        services.ConfigureAndValidate<TokensConfiguration.InfrastructureConfiguration>(configuration.GetSection("Infrastructure").Bind);

        services.AddPersistence();
        return services;
    }
}
