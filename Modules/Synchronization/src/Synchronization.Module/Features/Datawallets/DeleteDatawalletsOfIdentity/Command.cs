using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.Datawallets.DeleteDatawalletsOfIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteDatawalletsOfIdentityCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
