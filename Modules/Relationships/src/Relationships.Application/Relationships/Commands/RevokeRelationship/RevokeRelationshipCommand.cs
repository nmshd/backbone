using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RevokeRelationship;

public class RevokeRelationshipCommand : IRequest<RevokeRelationshipResponse>
{
    public required string RelationshipId { get; set; }
}
