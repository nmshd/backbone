using Backbone.Synchronization.Domain.Entities.Sync;
using MediatR;

namespace Backbone.Synchronization.Application.SyncRuns.Commands.RefreshExpirationTimeOfSyncRun;

public class RefreshExpirationTimeOfSyncRunCommand : IRequest<RefreshExpirationTimeOfSyncRunResponse>
{
    public RefreshExpirationTimeOfSyncRunCommand(SyncRunId syncRunId)
    {
        SyncRunId = syncRunId;
    }

    public SyncRunId SyncRunId { get; set; }
}
