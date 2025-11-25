using System.Data;
using System.Data.Common;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Extensions;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Domain.Entities.Relationships;
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

    public SynchronizationDbContext(DbContextOptions<SynchronizationDbContext> options, IEventBus eventBus) : base(options, eventBus)
    {
    }

    public SynchronizationDbContext(DbContextOptions<SynchronizationDbContext> options, IServiceProvider serviceProvider, IEventBus eventBus) : base(options, eventBus, serviceProvider)
    {
    }

    public DbSet<Datawallet> Datawallets { get; set; } = null!;
    public DbSet<DatawalletModification> DatawalletModifications { get; set; } = null!;
    public DbSet<ExternalEvent> ExternalEvents { get; set; } = null!;
    public DbSet<SyncRun> SyncRuns { get; set; } = null!;
    public DbSet<SyncError> SyncErrors { get; set; } = null!;
    public DbSet<Relationship> Relationships { get; set; } = null!;

    public async Task<DbPaginationResult<DatawalletModification>> GetDatawalletModifications(IdentityAddress activeIdentity, long? localIndex, PaginationFilter paginationFilter,
        CancellationToken cancellationToken)
    {
        IQueryable<DatawalletModification> query;

        localIndex ??= -1;

        if (Database.IsNpgsql())
        {
            var activeIdentityParam = new NpgsqlParameter("createdBy", NpgsqlDbType.Char, IdentityAddress.MAX_LENGTH, "", ParameterDirection.Input, false, 0, 0, DataRowVersion.Default,
                activeIdentity.Value);
            var localIndexParam = new NpgsqlParameter("localIndex", localIndex);
            query = DatawalletModifications.FromSqlInterpolated(
                $"""SELECT * FROM(SELECT *, ROW_NUMBER() OVER(PARTITION BY "ObjectIdentifier", "Type", "PayloadCategory" ORDER BY "Index" DESC) AS rank FROM "Synchronization"."DatawalletModifications" m1 WHERE "CreatedBy" = {activeIdentityParam} AND "Index" > {localIndexParam}) AS ignoreDuplicates WHERE rank = 1""");
        }
        else if (Database.IsSqlServer())
        {
            var activeIdentityParam = new SqlParameter("createdBy", SqlDbType.Char, IdentityAddress.MAX_LENGTH, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default,
                activeIdentity.Value);
            var localIndexParam = new SqlParameter("localIndex", localIndex);
            query = DatawalletModifications.FromSqlInterpolated(
                $"SELECT * FROM(SELECT *, ROW_NUMBER() OVER(PARTITION BY ObjectIdentifier, Type, PayloadCategory ORDER BY [Index] DESC) AS rank FROM [Synchronization].[DatawalletModifications] m1 WHERE CreatedBy = {activeIdentityParam} AND [Index] > {localIndexParam}) AS ignoreDuplicates WHERE rank = 1");
        }
        else // Sqlite
        {
            var activeIdentityParam = new SqliteParameter("createdBy", activeIdentity.Value);
            var localIndexParam = new SqliteParameter("localIndex", localIndex);
            query = DatawalletModifications.FromSqlInterpolated(
                $"SELECT * FROM(SELECT *, ROW_NUMBER() OVER(PARTITION BY ObjectIdentifier, Type, PayloadCategory ORDER BY [Index] DESC) AS rank FROM [DatawalletModifications] m1 WHERE CreatedBy = {activeIdentityParam} AND [Index] > {localIndexParam}) AS ignoreDuplicates WHERE rank = 1");
        }

        return await query
            .AsNoTracking()
            .OrderAndPaginate(m => m.Index, paginationFilter, cancellationToken);
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

    public async Task CreateExternalEvent(ExternalEvent externalEvent)
    {
        await RunInTransaction(async () =>
        {
            var nextIndex = await GetNextExternalEventIndexForIdentity(externalEvent.Owner);

            // if the transaction is retried, it usually means that there was a race condition, which means
            // that we have to recalculate the Index to avoid getting the same race condition again.
            externalEvent.UpdateIndex(nextIndex);

            await Set<ExternalEvent>().AddAsync(externalEvent);

            await SaveChangesAsync(CancellationToken.None);
        }, [DbErrorCodes.SQLSERVER_INDEX_ALREADY_EXISTS, DbErrorCodes.POSTGRES_INDEX_ALREADY_EXISTS]);
    }

    public async Task DeleteUnsyncedExternalEventsWithOwnerAndContext(IdentityAddress owner, string context)
    {
        await Set<ExternalEvent>()
            .Unsynced()
            .WithOwner(owner)
            .WithContext(context)
            .ExecuteDeleteAsync();
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
            .AsSplitQuery()
            .CreatedBy(createdBy)
            .Where(s => s.Id == syncRunId)
            .Include(s => s.ExternalEvents)
            .ThenInclude(e => e.Errors)
            .GetFirst(cancellationToken);
    }

    public async Task<SyncRun?> GetPreviousSyncRunWithExternalEvents(IdentityAddress createdBy, CancellationToken cancellationToken)
    {
        var previousSyncRun = await SyncRuns
            .AsSplitQuery()
            .Include(s => s.ExternalEvents)
            .ThenInclude(e => e.Errors)
            .CreatedBy(createdBy)
            .OrderByDescending(s => s.Index)
            .FirstOrDefaultAsync(cancellationToken);

        return previousSyncRun;
    }

    public async Task<bool> DoNewUnsyncedExternalEventsExist(IdentityAddress owner, byte maxErrorCount, CancellationToken cancellationToken)
    {
        return await UnsyncedExternalEventsQuery(owner, maxErrorCount).AnyAsync(cancellationToken);
    }

    public async Task<List<ExternalEvent>> GetUnsyncedExternalEvents(IdentityAddress owner, byte maxErrorCount, CancellationToken cancellationToken)
    {
        var unsyncedEvents = await UnsyncedExternalEventsQuery(owner, maxErrorCount).ToListAsync(cancellationToken);

        return unsyncedEvents;
    }

    private IQueryable<ExternalEvent> UnsyncedExternalEventsQuery(IdentityAddress owner, byte maxErrorCount)
    {
        return ExternalEvents
            .WithOwner(owner)
            .Unsynced()
            .NotBlocked()
            .WithErrorCountBelow(maxErrorCount);
    }

    public async Task DeleteBlockedExternalEventsWithTypeAndContext(ExternalEventType type, string context, CancellationToken cancellationToken)
    {
        await ExternalEvents
            .Blocked()
            .WithType(type)
            .WithContext(context)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<DbPaginationResult<ExternalEvent>> GetExternalEventsOfSyncRun(PaginationFilter paginationFilter, IdentityAddress owner, SyncRunId syncRunId, CancellationToken cancellationToken)
    {
        var query = await ExternalEvents
            .WithOwner(owner)
            .AssignedToSyncRun(syncRunId)
            .OrderAndPaginate(x => x.Index, paginationFilter, cancellationToken);

        return query;
    }

    public async Task<List<ExternalEvent>> GetBlockedExternalEventsWithTypeAndContext(ExternalEventType type, string context, CancellationToken cancellationToken)
    {
        var query = await ExternalEvents
            .Blocked()
            .WithType(type)
            .WithContext(context)
            .ToListAsync(cancellationToken);

        return query;
    }

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
        configurationBuilder.Properties<RelationshipId>().AreUnicode(false).AreFixedLength().HaveMaxLength(RelationshipId.MAX_LENGTH).HaveConversion<RelationshipIdEntityFrameworkValueConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Synchronization");

        builder.ApplyConfigurationsFromAssembly(typeof(SynchronizationDbContext).Assembly);
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
}
