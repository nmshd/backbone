using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeRelationship;

public class DecomposeRelationshipCommand : IRequest<DecomposeRelationshipResponse>
{
    public required string RelationshipId { get; init; }
}
