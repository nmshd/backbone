using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.DeleteDatawalletsOfIdentity;

public class DeleteDatawalletsOfIdentityCommand : IRequest
{
    public DeleteDatawalletsOfIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
