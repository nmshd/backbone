using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.DeleteSyncRunsOfIdentity;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
