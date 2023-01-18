using MediatR;
using Synchronization.Domain.Entities.Sync;

namespace Synchronization.Application.SyncRuns.Commands.RefreshExpirationTimeOfSyncRun;

public class RefreshExpirationTimeOfSyncRunCommand : IRequest<RefreshExpirationTimeOfSyncRunResponse>
{
    public RefreshExpirationTimeOfSyncRunCommand(SyncRunId syncRunId)
    {
        SyncRunId = syncRunId;
    }

    public SyncRunId SyncRunId { get; set; }
}
