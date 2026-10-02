using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.ExecuteHousekeeping;

public class Handler : IRequestHandler<Command>
{
    private readonly ISynchronizationDbContext _dbContext;

    public Handler(ISynchronizationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await DeleteSyncRuns(cancellationToken);
        await DeleteDatawalletModifications(cancellationToken);
    }

    private async Task DeleteSyncRuns(CancellationToken cancellationToken)
    {
        await HousekeepingTelemetry.TrackItemDeletion("sync runs", ct => _dbContext.Set<SyncRun>().Where(SyncRun.CanBeCleanedUp).ExecuteDeleteAsync(ct), cancellationToken);
    }

    private async Task DeleteDatawalletModifications(CancellationToken cancellationToken)
    {
        await HousekeepingTelemetry.TrackItemDeletion("datawallet modifications",
            ct => _dbContext.Set<DatawalletModification>().Where(DatawalletModification.CanBeCleanedUp).ExecuteDeleteAsync(ct), cancellationToken);
    }
}
