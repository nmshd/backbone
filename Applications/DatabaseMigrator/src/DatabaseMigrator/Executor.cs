using System.Data.SqlClient;
using System.Reflection;
using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.Attributes;
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

    private readonly Dictionary<Type, ModuleType> _modules = new()
    {
        { typeof(ChallengesDbContext), ModuleType.Challenges }, { typeof(DevicesDbContext), ModuleType.Devices },
        { typeof(FilesDbContext), ModuleType.Files }, { typeof(RelationshipsDbContext), ModuleType.Relationships },
        { typeof(QuotasDbContext), ModuleType.Quotas }, { typeof(MessagesDbContext), ModuleType.Messages },
        { typeof(SynchronizationDbContext), ModuleType.Synchronization }, { typeof(TokensDbContext), ModuleType.Tokens },
        { typeof(AdminApiDbContext), ModuleType.AdminApi }
    };

    private readonly List<Type> _moduleContextTypes =
    [
        typeof(AdminApiDbContext),
        typeof(ChallengesDbContext),
        typeof(DevicesDbContext),
        typeof(FilesDbContext),
        typeof(MessagesDbContext),
        typeof(QuotasDbContext),
        typeof(RelationshipsDbContext),
        typeof(SynchronizationDbContext),
        typeof(TokensDbContext)
    ];

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
        _logger.StartReadingMigrations();

        IReadOnlyList<MigrationId> migrationList = [];

        try
        {
            var tree = await ReadMigrations();
            migrationList = tree.MigrationSequence;
        }
        catch (Exception ex)
        {
            _logger.ErrorWhileReadingMigrations(ex);
            Environment.Exit(1);
        }

        _logger.SuccessfullyReadMigrations();

        _logger.StartApplyingMigrations();

        try
        {
            foreach (var info in migrationList) await MigrateDbContext(_moduleContextTypes[(int)info.Type], info.Id);
        }
        catch (Exception ex)
        {
            _logger.ErrorWhileApplyingMigrations(ex);
            await RollbackAlreadyMigratedDbContexts();
            Environment.Exit(1);
        }

        _logger.SuccessfullyAppliedMigrations();
    }

    private async Task<GraphHandler> ReadMigrations()
    {
        List<MigrationInfo> migrations = [];

        foreach (var (type, moduleType) in _modules)
        {
            var context = _serviceProvider.GetDbContext(type);
            var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            var dependencies = LoadMigrationDependencies(type, context.Database.IsSqlServer());

            migrations.AddRange(appliedMigrations.Select(id => new MigrationInfo(new MigrationId(moduleType, id), true, dependencies[id])));
            migrations.AddRange(pendingMigrations.Select(id => new MigrationInfo(new MigrationId(moduleType, id), false, dependencies[id])));
        }

        return new GraphHandler(migrations);
    }

    private Dictionary<string, IList<MigrationId>> LoadMigrationDependencies(Type dbContextType, bool useSqlServer)
    {
        var assemblyNameSuffix = useSqlServer ? "SqlServer" : "Postgres";
        var assembly = Assembly.Load(new AssemblyName($"{dbContextType.Assembly.GetName().Name}.Database.{assemblyNameSuffix}"));
        var definedTypes = assembly.DefinedTypes
            .Where(t => t.BaseType == typeof(Migration));
        Dictionary<string, IList<MigrationId>> ret = [];

        foreach (var type in definedTypes)
        {
            var idAttr = type.GetCustomAttribute<MigrationAttribute>();
            if (idAttr == null) continue;

            var dependencies = type.GetCustomAttributes<DependsOnAttribute>()
                .Select(attr => new MigrationId(attr.Module, attr.MigrationId))
                .ToList();

            ret[idAttr.Id] = dependencies;
        }

        return ret;
    }

    private async Task MigrateDbContext(Type contextType, string? targetMigration = null)
    {
        await using var context = _serviceProvider.GetDbContext(contextType);

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
