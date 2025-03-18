using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Database;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Challenges.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, DatabaseConfiguration configuration)
    {
        services.AddDbContextForModule<ChallengesDbContext>(configuration, "Challenges");

        services.AddScoped<IChallengesRepository, ChallengesRepository>();
    }
}
