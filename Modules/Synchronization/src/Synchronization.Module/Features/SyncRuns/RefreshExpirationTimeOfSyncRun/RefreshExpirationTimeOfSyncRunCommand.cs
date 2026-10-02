using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.RefreshExpirationTimeOfSyncRun;

public class RefreshExpirationTimeOfSyncRunCommand : IRequest<RefreshExpirationTimeOfSyncRunResponse>
{
    public required string SyncRunId { get; init; }
}
