using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteSyncRunsOfIdentity;

public class DeleteSyncRunsOfIdentityCommand : IRequest
{
    public DeleteSyncRunsOfIdentityCommand(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; set; }
}
