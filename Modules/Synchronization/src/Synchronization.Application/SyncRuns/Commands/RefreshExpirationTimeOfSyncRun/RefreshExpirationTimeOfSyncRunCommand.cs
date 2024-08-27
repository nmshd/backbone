using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.RefreshExpirationTimeOfSyncRun;

public class RefreshExpirationTimeOfSyncRunCommand : IRequest<RefreshExpirationTimeOfSyncRunResponse>
{
    public RefreshExpirationTimeOfSyncRunCommand(string syncRunId)
    {
        SyncRunId = syncRunId;
    }

    public string SyncRunId { get; set; }
}
