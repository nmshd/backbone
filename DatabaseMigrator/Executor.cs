using System.Data.SqlClient;
using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using Polly;

namespace Backbone.DatabaseMigrator;

public class Executor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Executor> _logger;

    public Executor(IServiceProvider serviceProvider, ILogger<Executor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Execute()
    {
        _logger.LogInformation("Starting migrations...");

        await MigrateDbContext<ChallengesDbContext>(_serviceProvider);
        await MigrateDbContext<DevicesDbContext>(_serviceProvider);
        await MigrateDbContext<FilesDbContext>(_serviceProvider);
        await MigrateDbContext<MessagesDbContext>(_serviceProvider);
        await MigrateDbContext<QuotasDbContext>(_serviceProvider);
        await MigrateDbContext<RelationshipsDbContext>(_serviceProvider);
        await MigrateDbContext<SynchronizationDbContext>(_serviceProvider);
        await MigrateDbContext<TokensDbContext>(_serviceProvider);
        await MigrateDbContext<AdminApiDbContext>(_serviceProvider);

        _logger.LogInformation("Migrations successfully applied");
    }

    private async Task MigrateDbContext<TContext>(IServiceProvider serviceProvider, string? targetMigration = null) where TContext : DbContext
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Migrating database associated with context '{context}' to target migration '{targetMigration}'", typeof(TContext).Name, targetMigration);

            var retry = Policy.Handle<SqlException>().Or<PostgresException>()
                .WaitAndRetry([
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(15)
                ]);

            var migrator = context.GetInfrastructure().GetRequiredService<IMigrator>();

            await retry.Execute(() => migrator.MigrateAsync(targetMigration));

            logger.LogInformation("Migrated database associated with context '{context}' to target migration '{targetMigration}'", typeof(TContext).Name, targetMigration);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database associated with context {context} to target migration '{targetMigration}'", typeof(TContext).Name, targetMigration);
            throw;
        }
    }
}
