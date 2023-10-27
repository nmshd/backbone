using AdminUi.Configuration;
using Backbone.Modules.Relationships.Application.Extensions;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;

namespace AdminUi.Extensions;

public static class RelationshipsServiceCollectionExtensions
{
    public static IServiceCollection AddRelationships(this IServiceCollection services,
        RelationshipsConfiguration configuration)
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
