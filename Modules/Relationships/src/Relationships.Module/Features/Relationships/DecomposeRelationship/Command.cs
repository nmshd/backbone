using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.DecomposeRelationship;

public class Command : IRequest<Response>
{
    public required string RelationshipId { get; init; }
}
