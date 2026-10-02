using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.Datawallets.DeleteDatawalletsOfIdentity;

public class DeleteDatawalletsOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
