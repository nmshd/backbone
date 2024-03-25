using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RevokeRelationship;

public class RevokeRelationshipCommand : IRequest<RevokeRelationshipResponse>
{
    public string RelationshipId { get; set; }
}
