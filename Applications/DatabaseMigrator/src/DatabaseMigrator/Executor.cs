using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using Polly;
using Polly.Retry;

namespace Backbone.DatabaseMigrator;

public class Executor
{
    private readonly DbContextProvider _dbContextProvider;
    private readonly MigrationReader _migrationReader;
    private readonly ILogger<Executor> _logger;

    private readonly Stack<(Type dbContextType, string? migrationBeforeChanges)> _migratedDbContexts = new();
    private readonly RetryPolicy _retryPolicy;

    public Executor(DbContextProvider dbContextProvider, MigrationReader migrationReader, ILogger<Executor> logger)
    {
        _dbContextProvider = dbContextProvider;
        _migrationReader = migrationReader;
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
        var migrations = await ReadMigrations();

        try
        {
            await ApplyMigrations(migrations);
        }
        catch (Exception ex)
        {
            _logger.ErrorWhileApplyingMigrations(ex);
            await RollbackAlreadyMigratedDbContexts();
            Environment.Exit(1);
        }
    }

    private async Task<IEnumerable<Migration>> ReadMigrations()
    {
        _logger.StartReadingMigrations();

        IEnumerable<Migration> migrations = [];

        try
        {
            migrations = await _migrationReader.ReadMigrations();
        }
        catch (Exception ex)
        {
            _logger.ErrorWhileReadingMigrations(ex);
            Environment.Exit(1);
        }

        _logger.SuccessfullyReadMigrations();

        return migrations;
    }

    private async Task ApplyMigrations(IEnumerable<Migration> migrations)
    {
        _logger.StartApplyingMigrations();

        foreach (var migration in migrations)
            await MigrateDbContext(migration.DbContextType, migration.Name);

        _logger.SuccessfullyAppliedMigrations();
    }

    private async Task MigrateDbContext(Type contextType, string? targetMigration = null)
    {
        await using var context = _dbContextProvider.GetDbContext(contextType);

        if (targetMigration == null)
            _logger.MigratingDbContextToLatestMigration(contextType.Name);
        else
            _logger.MigratingDbContextToTargetMigration(contextType.Name, targetMigration);

        var migrationBeforeChanges = await context.GetLastAppliedMigration();

        try
        {
            await _retryPolicy.Execute(() => context.MigrateTo(targetMigration));
        }
        catch (Exception ex)
        {
            if (targetMigration == null)
                _logger.ErrorWhileApplyingMigrationToLatestMigration(ex, contextType.Name);
            else
                _logger.ErrorWhileApplyingMigrationToTargetMigration(ex, contextType.Name, targetMigration);
            throw;
        }
        finally
        {
            // we have to add the context even in case of an exception, because only some of the migrations might have been applied
            _migratedDbContexts.Push((context.GetType(), migrationBeforeChanges));
        }

        if (targetMigration == null)
            _logger.SuccessfullyMigratedDbContextToLatestMigration(contextType.Name);
        else
            _logger.SuccessfullyMigratedDbContextToTargetMigration(contextType.Name, targetMigration);
    }

    private async Task RollbackAlreadyMigratedDbContexts()
    {
        while (_migratedDbContexts.Count != 0)
        {
            var (contextType, migrationBeforeChanges) = _migratedDbContexts.Pop();

            await using var context = _dbContextProvider.GetDbContext(contextType);

            // if the migrationBeforeChanges is null, this means that it was the first migration; EF Core doesn't allow un-applying the first migration. But it's no problem because 
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
        }
    }
}

file static class Extensions
{
    extension(DbContext dbContext)
    {
        public async Task MigrateTo(string? targetMigration = null)
        {
            var migrator = dbContext.GetInfrastructure().GetRequiredService<IMigrator>();
            await migrator.MigrateAsync(targetMigration);
        }

        public async Task<string?> GetLastAppliedMigration()
        {
            var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync();
            return appliedMigrations.LastOrDefault();
        }
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
        EventName = "DatabaseMigrator.Executor.MigratingDbContextToLatestMigration",
        Level = LogLevel.Information,
        Message = "Migrating database associated with context '{context}' to the latest migration...")]
    public static partial void MigratingDbContextToLatestMigration(this ILogger logger, string context);

    [LoggerMessage(
        EventId = 561104,
        EventName = "DatabaseMigrator.Executor.MigratingDbContextToTargetMigration",
        Level = LogLevel.Information,
        Message = "Migrating database associated with context '{context}' to target migration '{targetMigration}'...")]
    public static partial void MigratingDbContextToTargetMigration(this ILogger logger, string context, string targetMigration);

    [LoggerMessage(
        EventId = 561105,
        EventName = "DatabaseMigrator.Executor.SuccessfullyMigratedDbContextToLatestMigration",
        Level = LogLevel.Information,
        Message = "Migrating database associated with context '{context}' to the latest migration...")]
    public static partial void SuccessfullyMigratedDbContextToLatestMigration(this ILogger logger, string context);

    [LoggerMessage(
        EventId = 561106,
        EventName = "DatabaseMigrator.Executor.SuccessfullyMigratedDbContextToTargetMigration",
        Level = LogLevel.Information,
        Message = "Migrating database associated with context '{context}' to target migration '{targetMigration}'...")]
    public static partial void SuccessfullyMigratedDbContextToTargetMigration(this ILogger logger, string context, string targetMigration);

    [LoggerMessage(
        EventId = 561108,
        EventName = "DatabaseMigrator.Executor.ErrorWhileApplyingMigrationToLatestMigration",
        Level = LogLevel.Critical,
        Message =
            "An error occurred while migrating the database associated with context '{context}' to the latest migration'")]
    public static partial void ErrorWhileApplyingMigrationToLatestMigration(this ILogger logger, Exception ex, string context);

    [LoggerMessage(
        EventId = 561107,
        EventName = "DatabaseMigrator.Executor.ErrorWhileApplyingMigrationToTargetMigration",
        Level = LogLevel.Critical,
        Message =
            "An error occurred while migrating the database associated with context '{context}' to target migration '{targetMigration}'")]
    public static partial void ErrorWhileApplyingMigrationToTargetMigration(this ILogger logger, Exception ex, string context, string targetMigration);

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

    [LoggerMessage(
        EventId = 561111,
        EventName = "DatabaseMigrator.Executor.StartReadingMigrations",
        Level = LogLevel.Information,
        Message = "Start reading migrations...")]
    public static partial void StartReadingMigrations(this ILogger logger);

    [LoggerMessage(
        EventId = 561112,
        EventName = "DatabaseMigrator.Executor.SuccessfullyReadMigrations",
        Level = LogLevel.Information,
        Message = "All migrations were successfully read")]
    public static partial void SuccessfullyReadMigrations(this ILogger logger);

    [LoggerMessage(
        EventId = 561113,
        EventName = "DatabaseMigrator.Executor.ErrorWhileReadingMigrations",
        Level = LogLevel.Critical,
        Message = "An error occurred while reading the migrations. No changes will be made")]
    public static partial void ErrorWhileReadingMigrations(this ILogger logger, Exception ex);
}
