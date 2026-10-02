using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.DeleteSyncRunsOfIdentity;

public class DeleteSyncRunsOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
