using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.DeleteDatawalletsOfIdentity;

public class DeleteDatawalletsOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
