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
using Polly.Retry;

namespace Backbone.DatabaseMigrator;

public class Executor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Executor> _logger;

    private readonly Stack<(Type dbContextType, string? migrationBeforeChanges)> _migratedDbContexts = new();
    private readonly RetryPolicy _retryPolicy;

    public Executor(IServiceProvider serviceProvider, ILogger<Executor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _retryPolicy = Policy.Handle<SqlException>().Or<PostgresException>()
            .WaitAndRetry([
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15)
            ]);
    }

    public async Task Execute()
    {
        _logger.StartApplyingMigrations();

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
            _logger.ErrorWhileApplyingMigrations(ex);
            await RollbackAlreadyMigratedDbContexts();
            Environment.Exit(1);
        }

        _logger.SuccessfullyAppliedMigrations();
    }

    private async Task MigrateDbContext<TContext>(string? targetMigration = null) where TContext : DbContext
    {
        await using var context = _serviceProvider.GetDbContext<TContext>();

        if (targetMigration == null)
            _logger.MigratingDbContext(typeof(TContext).Name);
        else
            _logger.MigratingDbContext(typeof(TContext).Name, targetMigration);

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (!pendingMigrations.Any())
            return;

        var migrationBeforeChanges = await context.GetLastAppliedMigration();

        try
        {
            await _retryPolicy.Execute(() => context.MigrateTo(targetMigration));
        }
        catch (Exception ex)
        {
            if (targetMigration == null)
                _logger.ErrorWhileApplyingMigration(ex, typeof(TContext).Name);
            else
                _logger.ErrorWhileApplyingMigration(ex, typeof(TContext).Name, targetMigration);
            throw;
        }
        finally
        {
            _migratedDbContexts.Push((context.GetType(), migrationBeforeChanges));
        }

        if (targetMigration == null)
            _logger.SuccessfullyMigratedDbContext(typeof(TContext).Name);
        else
            _logger.SuccessfullyMigratedDbContext(typeof(TContext).Name, targetMigration);
    }

    private async Task RollbackAlreadyMigratedDbContexts()
    {
        while (_migratedDbContexts.Count != 0)
        {
            var (contextType, migrationBeforeChanges) = _migratedDbContexts.Peek();

            await using var context = _serviceProvider.GetDbContext(contextType);

            // if the migrationBeforeChanges is null, this means that it was the first migration; EF Core doesn't allow unapplying the first migration. But it's no problem because 
            // if there is only one migration, it means that in case of a rollback of the application version, there will be nothing that needs the tables from the first migration. 
            if (migrationBeforeChanges == null)
                continue;

            _logger.RollingBackDbContext(contextType.Name, migrationBeforeChanges);

            try
            {
                await context.MigrateTo(migrationBeforeChanges);
            }
            catch (Exception ex)
            {
                var remainingDbContexts = string.Join(", ", _migratedDbContexts.Select(c => c.dbContextType.Name));

                _logger.ErrorOnRollback(ex, context.GetType().Name, migrationBeforeChanges, remainingDbContexts);
            }

            _migratedDbContexts.Pop();
        }
    }
}

file static class Extensions
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

    public static DbContext GetDbContext<T>(this IServiceProvider serviceProvider) where T : DbContext
    {
        return serviceProvider.GetDbContext(typeof(T));
    }

    public static DbContext GetDbContext(this IServiceProvider serviceProvider, Type type)
    {
        var scope = serviceProvider.CreateScope();
        var context = (DbContext)scope.ServiceProvider.GetRequiredService(type);
        return context;
    }
}

internal static partial class ExecutorLogs
{
    [LoggerMessage(
        EventId = 561100,
        EventName = "DatabaseMigrator.Executor.StartApplyingMigrations",
        Level = LogLevel.Information,
        Message = "Start applying migrations...")]
    public static partial void StartApplyingMigrations(this ILogger logger);

    [LoggerMessage(
        EventId = 561101,
        EventName = "DatabaseMigrator.Executor.SuccessfullyAppliedMigrations",
        Level = LogLevel.Information,
        Message = "All migrations were successfully applied")]
    public static partial void SuccessfullyAppliedMigrations(this ILogger logger);

    [LoggerMessage(
        EventId = 561102,
        EventName = "DatabaseMigrator.Executor.ErrorWhileApplyingMigrations",
        Level = LogLevel.Critical,
        Message = "An error occurred while migrating the database. Rolling back already migrated DbContexts...")]
    public static partial void ErrorWhileApplyingMigrations(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 561103,
        EventName = "DatabaseMigrator.Executor.MigratingDbContext",
        Level = LogLevel.Information,
        Message = "Migrating database associated with context '{context}' to the latest migration...")]
    public static partial void MigratingDbContext(this ILogger logger, string context);

    [LoggerMessage(
        EventId = 561104,
        EventName = "DatabaseMigrator.Executor.MigratingDbContext",
        Level = LogLevel.Information,
        Message = "Migrating database associated with context '{context}' to target migration '{targetMigration}'...")]
    public static partial void MigratingDbContext(this ILogger logger, string context, string targetMigration);

    [LoggerMessage(
        EventId = 561105,
        EventName = "DatabaseMigrator.Executor.SuccessfullyMigratedDbContext",
        Level = LogLevel.Information,
        Message = "Migrating database associated with context '{context}' to the latest migration...")]
    public static partial void SuccessfullyMigratedDbContext(this ILogger logger, string context);

    [LoggerMessage(
        EventId = 561106,
        EventName = "DatabaseMigrator.Executor.SuccessfullyMigratedDbContext",
        Level = LogLevel.Information,
        Message = "Migrating database associated with context '{context}' to target migration '{targetMigration}'...")]
    public static partial void SuccessfullyMigratedDbContext(this ILogger logger, string context, string targetMigration);

    [LoggerMessage(
        EventId = 561107,
        EventName = "DatabaseMigrator.Executor.ErrorWhileApplyingMigration",
        Level = LogLevel.Critical,
        Message =
            "An error occurred while migrating the database associated with context '{context}' to target migration '{targetMigration}'")]
    public static partial void ErrorWhileApplyingMigration(this ILogger logger, Exception ex, string context, string targetMigration);

    [LoggerMessage(
        EventId = 561108,
        EventName = "DatabaseMigrator.Executor.ErrorWhileApplyingMigration",
        Level = LogLevel.Critical,
        Message =
            "An error occurred while migrating the database associated with context '{context}' to the latest migration'")]
    public static partial void ErrorWhileApplyingMigration(this ILogger logger, Exception ex, string context);

    [LoggerMessage(
        EventId = 561109,
        EventName = "DatabaseMigrator.Executor.RollingBackDbContext",
        Level = LogLevel.Information,
        Message =
            "Rolling back context '{context}' to migration '{migrationBeforeChanges}'....")]
    public static partial void RollingBackDbContext(this ILogger logger, string context, string migrationBeforeChanges);

    [LoggerMessage(
        EventId = 561110,
        EventName = "DatabaseMigrator.Executor.ErrorOnRollback",
        Level = LogLevel.Critical,
        Message =
            "There was an error while rolling back the migration of context '{context}' to migration '{migrationBeforeChanges}'. The following DbContexts couldn't be rolled back: {remainingDbContexts}")]
    public static partial void ErrorOnRollback(this ILogger logger, Exception ex, string context, string migrationBeforeChanges, string remainingDbContexts);
}
