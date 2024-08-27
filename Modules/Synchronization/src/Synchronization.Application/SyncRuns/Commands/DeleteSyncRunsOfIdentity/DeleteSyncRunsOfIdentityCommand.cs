using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteSyncRunsOfIdentity;

public class DeleteSyncRunsOfIdentityCommand : IRequest
{
    public DeleteSyncRunsOfIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
