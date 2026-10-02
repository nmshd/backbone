using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.AcceptRelationshipReactivation;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("AcceptRelationshipReactivationCommand")]
public class Command : IRequest<Response>
{
    public required string RelationshipId { get; init; }
}
