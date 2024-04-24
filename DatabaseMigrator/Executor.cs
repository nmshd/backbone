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

    private readonly Stack<(Type dbContextType, string? migrationBeforeChanges)> _migratedDbContexts = new();

    public Executor(IServiceProvider serviceProvider, ILogger<Executor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Execute()
    {
        _logger.LogInformation("Starting migrations...");

        try
        {
            await MigrateDbContext<ChallengesDbContext>();
            await MigrateDbContext<DevicesDbContext>();
            await MigrateDbContext<FilesDbContext>();
            await MigrateDbContext<RelationshipsDbContext>();
            await MigrateDbContext<QuotasDbContext>();
            await MigrateDbContext<MessagesDbContext>();
            await MigrateDbContext<SynchronizationDbContext>();
            await MigrateDbContext<TokensDbContext>();
            await MigrateDbContext<AdminApiDbContext>();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An error occurred while migrating the database. Rolling back already migrated DbContexts...");
            await RollbackAlreadyMigratedDbContexts();
            Environment.Exit(1);
        }

        _logger.LogInformation("Migrations successfully applied");
    }

    private async Task MigrateDbContext<TContext>(string? targetMigration = null) where TContext : DbContext
    {
        using var scope = _serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        if (targetMigration == null)
            logger.LogInformation("Migrating database associated with context '{context}' to the latest migration...", typeof(TContext).Name);
        else
            logger.LogInformation("Migrating database associated with context '{context}' to target migration '{targetMigration}'...", typeof(TContext).Name, targetMigration);

        var retry = Policy.Handle<SqlException>().Or<PostgresException>()
            .WaitAndRetry([
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15)
            ]);

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (!pendingMigrations.Any())
            return;

        var migrationBeforeChanges = await context.GetLastAppliedMigration();

        try
        {
            await retry.Execute(() => context.MigrateTo(targetMigration));
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An error occurred while migrating the database associated with context '{context}' to target migration '{targetMigration}'", typeof(TContext).Name,
                targetMigration);
            throw;
        }
        finally
        {
            _migratedDbContexts.Push((context.GetType(), migrationBeforeChanges));
        }

        if (targetMigration == null)
            logger.LogInformation("Successfully migrated database associated with context '{context}' to the latest migration.", typeof(TContext).Name);
        else
            logger.LogInformation("Successfully migrated database associated with context '{context}' to target migration '{targetMigration}'", typeof(TContext).Name, targetMigration);
    }

    private async Task RollbackAlreadyMigratedDbContexts()
    {
        while (_migratedDbContexts.Count != 0)
        {
            var (contextType, migrationBeforeChanges) = _migratedDbContexts.Peek();
            var scope = _serviceProvider.CreateScope();
            var context = (DbContext)scope.ServiceProvider.GetRequiredService(contextType);

            // if the migration before changes is null, this means that it was the first migration; EF Core doesn't allow unapplying the first migration. But it's not problem because 
            // if there is only one migration, it means that in case of a rollback of the application version, there will be nothing that needs the tables from the first migration. 
            if (migrationBeforeChanges == null)
                continue;

            try
            {
                await context.MigrateTo(migrationBeforeChanges);
                _migratedDbContexts.Pop();
            }
            catch (Exception ex)
            {
                var remainingDbContexts = string.Join(", ", _migratedDbContexts.Select(c => c.dbContextType.Name));

                _logger.LogCritical(ex,
                    "There was an error while rolling back the migration of context '{context}' to migration '{migrationBeforeChanges}'. The following DbContexts couldn't be rolled back: {dbContexts}",
                    context.GetType().Name, migrationBeforeChanges, remainingDbContexts);
            }
        }
    }
}

file static class DbContextExtensions
{
    public static async Task MigrateTo(this DbContext dbContext, string? targetMigration = null)
    {
        var migrator = dbContext.GetInfrastructure().GetRequiredService<IMigrator>();
        await migrator.MigrateAsync(targetMigration);
    }

    public static async Task<string?> GetLastAppliedMigration(this DbContext dbContext)
    {
        var stateBeforeMigration = await dbContext.Database.GetAppliedMigrationsAsync();
        var migrationBeforeChanges = stateBeforeMigration.LastOrDefault();
        return migrationBeforeChanges;
    }
}
