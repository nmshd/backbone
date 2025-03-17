namespace Backbone.DatabaseMigrator;

public static class IServiceCollectionExtensions
{
    public static void AddAllDbContexts(this IServiceCollection services, SqlDatabaseConfiguration databaseConfiguration)
    {
        Modules.Announcements.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, options =>
        {
            options.Provider = databaseConfiguration.Provider;
            options.ConnectionString = databaseConfiguration.ConnectionString;
            options.CommandTimeout = databaseConfiguration.CommandTimeout;
        });

        Modules.Challenges.Infrastructure.Persistence.IServiceCollectionExtensions.AddDatabase(services, options =>
        {
            options.Provider = databaseConfiguration.Provider;
            options.DbConnectionString = databaseConfiguration.ConnectionString;
            options.CommandTimeout = databaseConfiguration.CommandTimeout;
        });

        Modules.Devices.Infrastructure.Persistence.IServiceCollectionExtensions.AddDatabase(services, options =>
        {
            options.Provider = databaseConfiguration.Provider;
            options.ConnectionString = databaseConfiguration.ConnectionString;
            options.CommandTimeout = databaseConfiguration.CommandTimeout;
        });

        Modules.Files.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, options =>
        {
            options.Provider = databaseConfiguration.Provider;
            options.ConnectionString = databaseConfiguration.ConnectionString;
            options.CommandTimeout = databaseConfiguration.CommandTimeout;
        });

        Modules.Messages.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, options =>
        {
            options.Provider = databaseConfiguration.Provider;
            options.ConnectionString = databaseConfiguration.ConnectionString;
            options.CommandTimeout = databaseConfiguration.CommandTimeout;
        });

        Modules.Quotas.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, options =>
        {
            options.Provider = databaseConfiguration.Provider;
            options.ConnectionString = databaseConfiguration.ConnectionString;
            options.CommandTimeout = databaseConfiguration.CommandTimeout;
        });

        Modules.Relationships.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, options =>
        {
            options.Provider = databaseConfiguration.Provider;
            options.ConnectionString = databaseConfiguration.ConnectionString;
            options.CommandTimeout = databaseConfiguration.CommandTimeout;
        });

        Modules.Synchronization.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, options =>
        {
            options.Provider = databaseConfiguration.Provider;
            options.ConnectionString = databaseConfiguration.ConnectionString;
            options.CommandTimeout = databaseConfiguration.CommandTimeout;
        });

        Modules.Tokens.Infrastructure.Persistence.Database.IServiceCollectionExtensions.AddDatabase(services, options =>
        {
            options.Provider = databaseConfiguration.Provider;
            options.ConnectionString = databaseConfiguration.ConnectionString;
            options.CommandTimeout = databaseConfiguration.CommandTimeout;
        });

        AdminApi.Infrastructure.Persistence.IServiceCollectionExtensions.AddDatabase(services, options =>
        {
            options.Provider = databaseConfiguration.Provider;
            options.ConnectionString = databaseConfiguration.ConnectionString;
            options.CommandTimeout = databaseConfiguration.CommandTimeout;
        });
    }
}
