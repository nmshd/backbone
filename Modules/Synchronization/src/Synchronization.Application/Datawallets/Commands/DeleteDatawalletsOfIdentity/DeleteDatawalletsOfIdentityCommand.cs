using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.DeleteDatawalletsOfIdentity;
public class DeleteDatawalletsOfIdentityCommand(IdentityAddress identityAddress) : IRequest
{
    public IdentityAddress IdentityAddress { get; set; } = identityAddress;
}
