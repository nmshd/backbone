using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Application.Infrastructure;

public interface ISynchronizationDbContext : IDbContext
{
    Task<DbPaginationResult<DatawalletModification>> GetDatawalletModifications(IdentityAddress activeIdentity,
        long? localIndex, PaginationFilter paginationFilter, CancellationToken cancellationToken);

    Task<Datawallet?> GetDatawalletForInsertion(IdentityAddress owner, CancellationToken cancellationToken);
    Task<Datawallet?> GetDatawallet(IdentityAddress owner, CancellationToken cancellationToken);
    Task<ExternalEvent> CreateExternalEvent(IdentityAddress owner, ExternalEventType type, object payload);
    Task<SyncRun> GetSyncRun(SyncRunId syncRunId, IdentityAddress createdBy, CancellationToken cancellationToken);
    Task<bool> IsActiveSyncRunAvailable(IdentityAddress createdBy, CancellationToken cancellationToken);

    Task<SyncRun> GetSyncRunAsNoTracking(SyncRunId syncRunId, IdentityAddress createdBy,
        CancellationToken cancellationToken);

    Task<SyncRun> GetSyncRunWithExternalEvents(SyncRunId syncRunId, IdentityAddress createdBy,
        CancellationToken cancellationToken);

    Task<SyncRun?> GetPreviousSyncRunWithExternalEvents(IdentityAddress createdBy, CancellationToken cancellationToken);

    Task<List<ExternalEvent>> GetUnsyncedExternalEvents(IdentityAddress owner, byte maxErrorCount,
        CancellationToken cancellationToken);

    Task<DbPaginationResult<ExternalEvent>> GetExternalEventsOfSyncRun(PaginationFilter paginationFilter,
        IdentityAddress owner, SyncRunId syncRunId, CancellationToken cancellationToken);
}
