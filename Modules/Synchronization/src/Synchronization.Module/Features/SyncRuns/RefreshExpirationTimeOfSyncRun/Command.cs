using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.RefreshExpirationTimeOfSyncRun;

public class Command : IRequest<Response>
{
    public required string SyncRunId { get; init; }
}
