using Backbone.API.Configuration;
using Challenges.Application.Extensions;
using Challenges.Infrastructure.Persistence;

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