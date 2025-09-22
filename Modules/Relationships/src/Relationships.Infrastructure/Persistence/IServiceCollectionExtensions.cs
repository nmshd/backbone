using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    public static void AddPersistence(this IServiceCollection services, DatabaseConfiguration options)
    {
        services.AddDatabase(options);

        services.AddTransient<IRelationshipsRepository, RelationshipsRepository>();
        services.AddTransient<IRelationshipTemplatesRepository, RelationshipTemplatesRepository>();
    }
}
