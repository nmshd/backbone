using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RejectRelationshipReactivation;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("RejectRelationshipReactivationCommand")]
public class Command : IRequest<Response>
{
    public required string RelationshipId { get; init; }
}
