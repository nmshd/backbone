using Backbone.AdminApi.Configuration;
using Backbone.Modules.Tokens.Application.Extensions;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Repository;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Extensions;

public static class TokensServiceCollectionExtensions
{
    public static IServiceCollection AddTokens(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication(configuration.GetSection("Application"));

        services.ConfigureAndValidate<TokensConfiguration.InfrastructureConfiguration>(configuration.GetSection("Infrastructure").Bind);

        var infrastructureConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<TokensConfiguration.InfrastructureConfiguration>>().Value;

        services.AddDatabase(options =>
        {
            options.Provider = infrastructureConfiguration.SqlDatabase.Provider;
            options.DbConnectionString = infrastructureConfiguration.SqlDatabase.ConnectionString;
        });

        // Note: Registration required. Only Modules have their used repositories registered in the DI container.
        services.AddTransient<ITokensRepository, TokensRepository>();

        return services;
    }
}
