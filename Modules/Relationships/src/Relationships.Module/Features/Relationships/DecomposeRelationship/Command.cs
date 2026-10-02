using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.DecomposeRelationship;

public class DecomposeRelationshipCommand : IRequest<DecomposeRelationshipResponse>
{
    public required string RelationshipId { get; init; }
}
