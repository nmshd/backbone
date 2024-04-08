using System.Data;
using System.Data.Common;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Extensions;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;

public class SynchronizationDbContext : AbstractDbContextBase, ISynchronizationDbContext
{
    public SynchronizationDbContext()
    {
    }

    public SynchronizationDbContext(DbContextOptions<SynchronizationDbContext> options) : base(options)
    {
    }

    public SynchronizationDbContext(DbContextOptions<SynchronizationDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
    {
    }

    public DbSet<Datawallet> Datawallets { get; set; } = null!;
    public DbSet<DatawalletModification> DatawalletModifications { get; set; } = null!;
    public DbSet<ExternalEvent> ExternalEvents { get; set; } = null!;
    public DbSet<SyncRun> SyncRuns { get; set; } = null!;
    public DbSet<SyncError> SyncErrors { get; set; } = null!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<DatawalletId>().AreUnicode(false).AreFixedLength().HaveMaxLength(DatawalletId.MAX_LENGTH).HaveConversion<DatawalletIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<Datawallet.DatawalletVersion>().AreUnicode(false).HaveConversion<DatawalletVersionEntityFrameworkValueConverter>();
        configurationBuilder.Properties<DatawalletModificationId>().AreUnicode(false).AreFixedLength().HaveMaxLength(DatawalletModificationId.MAX_LENGTH)
            .HaveConversion<DatawalletModificationIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<SyncRunId>().AreUnicode(false).AreFixedLength().HaveMaxLength(SyncRunId.MAX_LENGTH).HaveConversion<SyncRunIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<ExternalEventId>().AreUnicode(false).AreFixedLength().HaveMaxLength(ExternalEventId.MAX_LENGTH).HaveConversion<ExternalEventIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<SyncErrorId>().AreUnicode(false).AreFixedLength().HaveMaxLength(SyncErrorId.MAX_LENGTH).HaveConversion<SyncErrorIdEntityFrameworkValueConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(SynchronizationDbContext).Assembly);
    }

    public async Task<DbPaginationResult<DatawalletModification>> GetDatawalletModifications(IdentityAddress activeIdentity, long? localIndex, PaginationFilter paginationFilter,
        CancellationToken cancellationToken)
    {
        // Use DbParameter here in order to define the type of the activeIdentity parameter explicitly. Otherwise nvarchar(4000) is used, which causes performance problems.
        // (https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/brownfield/how-data-access-code-affects-database-performance)
        DbParameter activeIdentityParam;
        if (Database.IsSqlServer())
            activeIdentityParam = new SqlParameter("createdBy", SqlDbType.Char, IdentityAddress.MAX_LENGTH, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default,
                activeIdentity.StringValue);
        else if (Database.IsNpgsql())
            activeIdentityParam = new NpgsqlParameter("createdBy", NpgsqlDbType.Char, IdentityAddress.MAX_LENGTH, "", ParameterDirection.Input, false, 0, 0, DataRowVersion.Default,
                activeIdentity.StringValue);
        else
            activeIdentityParam = new SqliteParameter("createdBy", activeIdentity.StringValue);

        var paginationResult = Database.IsNpgsql()
            ? await DatawalletModifications
                .FromSqlInterpolated(
                    $"""SELECT * FROM(SELECT *, ROW_NUMBER() OVER(PARTITION BY "ObjectIdentifier", "Type", "PayloadCategory" ORDER BY "Index" DESC) AS rank FROM "Synchronization"."DatawalletModifications" m1 WHERE "CreatedBy" = {activeIdentityParam} AND "Index" > {localIndex ?? -1}) AS ignoreDuplicates WHERE rank = 1""")
                .AsNoTracking()
                .OrderAndPaginate(m => m.Index, paginationFilter, cancellationToken)
            : await DatawalletModifications
                .FromSqlInterpolated(
                    $"SELECT * FROM(SELECT *, ROW_NUMBER() OVER(PARTITION BY ObjectIdentifier, Type, PayloadCategory ORDER BY [Index] DESC) AS rank FROM [DatawalletModifications] m1 WHERE CreatedBy = {activeIdentityParam} AND [Index] > {localIndex ?? -1}) AS ignoreDuplicates WHERE rank = 1")
                .AsNoTracking()
                .OrderAndPaginate(m => m.Index, paginationFilter, cancellationToken);

        return paginationResult;
    }

    public async Task<Datawallet?> GetDatawalletForInsertion(IdentityAddress owner, CancellationToken cancellationToken)
    {
        var datawallet = await Datawallets
            .WithLatestModification(owner)
            .OfOwner(owner, cancellationToken);

        return datawallet;
    }

    public async Task<Datawallet?> GetDatawallet(IdentityAddress owner, CancellationToken cancellationToken)
    {
        var datawallet = await Datawallets
            .AsNoTracking()
            .OfOwner(owner, cancellationToken);
        return datawallet;
    }

    private async Task<long> GetNextExternalEventIndexForIdentity(IdentityAddress identity)
    {
        var latestIndex = await ExternalEvents
            .WithOwner(identity)
            .OrderByDescending(s => s.Index)
            .Select(s => (long?)s.Index)
            .FirstOrDefaultAsync();

        if (latestIndex == null)
            return 0;

        return latestIndex.Value + 1;
    }

    public async Task<ExternalEvent> CreateExternalEvent(IdentityAddress owner, ExternalEventType type, object payload)
    {
        ExternalEvent? externalEvent = null;

        await RunInTransaction(async () =>
        {
            if (externalEvent != null)
                // if the transaction is retried, the old event has to be removed from the DbSet, because a new one with a new index is added
                Set<ExternalEvent>().Remove(externalEvent);

            var nextIndex = await GetNextExternalEventIndexForIdentity(owner);
            externalEvent = new ExternalEvent(type, owner, nextIndex, payload);

            await ExternalEvents.AddAsync(externalEvent);
            await SaveChangesAsync(CancellationToken.None);
        }, [DbErrorCodes.SQLSERVER_INDEX_ALREADY_EXISTS, DbErrorCodes.POSTGRES_INDEX_ALREADY_EXISTS]);

        return externalEvent!;
    }

    public async Task<SyncRun> GetSyncRun(SyncRunId syncRunId, IdentityAddress createdBy, CancellationToken cancellationToken)
    {
        return await SyncRuns
            .CreatedBy(createdBy)
            .Where(s => s.Id == syncRunId)
            .GetFirst(cancellationToken);
    }

    public async Task<bool> IsActiveSyncRunAvailable(IdentityAddress createdBy, CancellationToken cancellationToken)
    {
        return await SyncRuns
            .AsNoTracking()
            .CreatedBy(createdBy)
            .NotFinalized()
            .Select(s => true)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<SyncRun> GetSyncRunAsNoTracking(SyncRunId syncRunId, IdentityAddress createdBy, CancellationToken cancellationToken)
    {
        return await SyncRuns
            .AsNoTracking()
            .CreatedBy(createdBy)
            .Where(s => s.Id == syncRunId)
            .GetFirst(cancellationToken);
    }

    public async Task<SyncRun> GetSyncRunWithExternalEvents(SyncRunId syncRunId, IdentityAddress createdBy, CancellationToken cancellationToken)
    {
        return await SyncRuns
            .CreatedBy(createdBy)
            .Where(s => s.Id == syncRunId)
            .Include(s => s.ExternalEvents)
            .GetFirst(cancellationToken);
    }

    public async Task<SyncRun?> GetPreviousSyncRunWithExternalEvents(IdentityAddress createdBy, CancellationToken cancellationToken)
    {
        var previousSyncRun = await SyncRuns
            .Include(s => s.ExternalEvents)
            .CreatedBy(createdBy)
            .OrderByDescending(s => s.Index)
            .FirstOrDefaultAsync(cancellationToken);

        return previousSyncRun;
    }

    public async Task<List<ExternalEvent>> GetUnsyncedExternalEvents(IdentityAddress owner, byte maxErrorCount, CancellationToken cancellationToken)
    {
        var unsyncedEvents = await ExternalEvents
            .WithOwner(owner)
            .Unsynced()
            .WithErrorCountBelow(maxErrorCount)
            .ToListAsync(cancellationToken);

        return unsyncedEvents;
    }

    public async Task<DbPaginationResult<ExternalEvent>> GetExternalEventsOfSyncRun(PaginationFilter paginationFilter, IdentityAddress owner, SyncRunId syncRunId, CancellationToken cancellationToken)
    {
        var query = await ExternalEvents
            .WithOwner(owner)
            .AssignedToSyncRun(syncRunId)
            .OrderAndPaginate(x => x.Index, paginationFilter, cancellationToken);

        return query;
    }
}
