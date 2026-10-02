using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.Datawallets.DeleteDatawalletsOfIdentity;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
