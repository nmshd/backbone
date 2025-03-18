using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Messages.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    public static void AddPersistence(this IServiceCollection services, DatabaseConfiguration options)
    {
        services.AddDatabase(options);

        services.AddTransient<IMessagesRepository, MessagesRepository>();
        services.AddTransient<IRelationshipsRepository, RelationshipsRepository>();
    }
}
