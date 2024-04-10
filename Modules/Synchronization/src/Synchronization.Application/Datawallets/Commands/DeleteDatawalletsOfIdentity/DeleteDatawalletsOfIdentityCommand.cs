using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.DeleteDatawalletsOfIdentity;
public class DeleteDatawalletsOfIdentityCommand : IRequest
{
    public DeleteDatawalletsOfIdentityCommand(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; set; }
}
