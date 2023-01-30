using Backbone.API.Configuration;
using Backbone.Modules.Challenges.Application.Extensions;
using Backbone.Modules.Challenges.Infrastructure.Persistence;

namespace Backbone.API.Extensions;

public static class ChallengesServiceCollectionExtensions
{
    public static IServiceCollection AddChallenges(this IServiceCollection services,
        ChallengesConfiguration configuration)
    {
        services.AddApplication();

        services.AddDatabase(dbOptions => dbOptions.DbConnectionString = configuration.Infrastructure.SqlDatabase.ConnectionString);

        return services;
    }
}