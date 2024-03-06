using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.RefreshExpirationTimeOfSyncRun;

public class RefreshExpirationTimeOfSyncRunCommand : IRequest<RefreshExpirationTimeOfSyncRunResponse>
{
    public RefreshExpirationTimeOfSyncRunCommand(SyncRunId syncRunId)
    {
        SyncRunId = syncRunId;
    }

    public SyncRunId SyncRunId { get; set; }
}
