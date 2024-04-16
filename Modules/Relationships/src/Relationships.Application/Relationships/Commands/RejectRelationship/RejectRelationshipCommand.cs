using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationship;

public class RejectRelationshipCommand : IRequest<RejectRelationshipResponse>
{
    public required string RelationshipId { get; set; }
}
