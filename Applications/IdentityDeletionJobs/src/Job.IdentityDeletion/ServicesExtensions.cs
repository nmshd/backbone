using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Exporters;
using Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Extractors;
using PostgresSqlExtractor = Backbone.Job.IdentityDeletion.IdentityDeletionVerifier.Extractors.PostgresSqlExtractor;

namespace Backbone.Job.IdentityDeletion;

public static class ServicesExtensions
{
    public static IServiceCollection RegisterIdentityDeleters(this IServiceCollection services)
    {
        services.AddTransient<IIdentityDeleter, Modules.Announcements.Application.Announcements.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Challenges.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Devices.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Files.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Messages.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Quotas.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Relationships.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Synchronization.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Tokens.Application.Identities.IdentityDeleter>();

        return services;
    }

    public static IServiceCollection RegisterDbExporterAndExtractor(this IServiceCollection services, IdentityDeletionJobConfiguration parsedConfiguration)
    {
        switch (parsedConfiguration.Infrastructure.SqlDatabase.Provider)
        {
            case DatabaseConfiguration.POSTGRES:
                services.AddTransient<IDbExporter, PostgresDbExporter>();
                services.AddTransient<ISqlExtractor, PostgresSqlExtractor>();
                break;

            case DatabaseConfiguration.SQLSERVER:
                services.AddTransient<IDbExporter, SqlServerDbExporter>();
                services.AddTransient<ISqlExtractor, SqlserverSqlExtractor>();
                break;

            default:
                throw new ArgumentOutOfRangeException($"No database provider registered for \"{parsedConfiguration.Infrastructure.SqlDatabase.Provider}\"");
        }

        return services;
    }
}
