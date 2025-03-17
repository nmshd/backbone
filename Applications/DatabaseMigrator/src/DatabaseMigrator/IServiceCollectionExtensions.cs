using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

namespace Backbone.DatabaseMigrator;

public static class IServiceCollectionExtensions
{
    public static void AddAllDbContexts(this IServiceCollection services, DatabaseConfiguration databaseConfiguration)
    {
        Modules.Announcements.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, databaseConfiguration);

        Modules.Challenges.Infrastructure.Persistence.IServiceCollectionExtensions.AddDatabase(services, databaseConfiguration);

        Modules.Devices.Infrastructure.Persistence.IServiceCollectionExtensions.AddDatabase(services, databaseConfiguration);

        Modules.Files.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, databaseConfiguration);

        Modules.Messages.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, databaseConfiguration);

        Modules.Quotas.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, databaseConfiguration);

        Modules.Relationships.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, databaseConfiguration);

        Modules.Synchronization.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, databaseConfiguration);

        Modules.Tokens.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, databaseConfiguration);

        AdminApi.Infrastructure.Persistence.IServiceCollectionExtensions.AddDatabase(services, databaseConfiguration);
    }
}
