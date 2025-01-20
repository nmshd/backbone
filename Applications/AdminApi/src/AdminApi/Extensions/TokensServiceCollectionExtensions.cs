using Backbone.AdminApi.Configuration;
using Backbone.Modules.Tokens.Application.Extensions;
using Backbone.Modules.Tokens.Infrastructure.Persistence;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Extensions;

public static class TokensServiceCollectionExtensions
{
    public static IServiceCollection AddTokens(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication(configuration.GetSection("Application"));

        services.ConfigureAndValidate<TokensConfiguration.InfrastructureConfiguration>(configuration.GetSection("Infrastructure").Bind);

        var infrastructureConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<TokensConfiguration.InfrastructureConfiguration>>().Value;

        services.AddPersistence(options =>
        {
            options.DbOptions.Provider = infrastructureConfiguration.SqlDatabase.Provider;
            options.DbOptions.DbConnectionString = infrastructureConfiguration.SqlDatabase.ConnectionString;
        });

        return services;
    }
}
