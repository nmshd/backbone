using MediatR;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteExternalEventsOfIdentity;

public class DeleteExternalEventsOfIdentityCommand : IRequest
{
    public DeleteExternalEventsOfIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
