using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.ReactivateRelationshipRequest;
public class ReactivateRelationshipRequestCommand : IRequest<ReactivateRelationshipRequestResponse>
{
    public required string RelationshipId { get; set; }
}
