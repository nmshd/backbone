using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.DecomposeRelationship;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DecomposeRelationshipCommand")]
public class Command : IRequest<Response>
{
    public required string RelationshipId { get; init; }
}
