using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.RefreshExpirationTimeOfSyncRun;

public class RefreshExpirationTimeOfSyncRunCommand : IRequest<RefreshExpirationTimeOfSyncRunResponse>
{
    public required string SyncRunId { get; init; }
}
