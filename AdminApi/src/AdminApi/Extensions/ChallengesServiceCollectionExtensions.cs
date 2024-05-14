using Backbone.AdminApi.Configuration;
using Backbone.Modules.Challenges.Application.Extensions;
using Backbone.Modules.Challenges.Infrastructure.Persistence;

namespace Backbone.AdminApi.Extensions;

public static class ChallengesServiceCollectionExtensions
{
    public static IServiceCollection AddChallenges(this IServiceCollection services,
        ChallengesConfiguration configuration)
    {
        services.AddApplication();

        services.AddDatabase(options =>
        {
            options.DbConnectionString = configuration.Infrastructure.SqlDatabase.ConnectionString;
            options.Provider = configuration.Infrastructure.SqlDatabase.Provider;
        });


        return services;
    }
}
