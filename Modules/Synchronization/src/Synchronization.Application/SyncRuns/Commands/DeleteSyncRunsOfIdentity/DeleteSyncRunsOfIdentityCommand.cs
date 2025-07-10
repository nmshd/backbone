using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteSyncRunsOfIdentity;

public class DeleteSyncRunsOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
