using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.ReactivateRelationship;
public class ReactivateRelationshipCommand : IRequest<ReactivateRelationshipResponse>
{
    public required string RelationshipId { get; set; }
}
