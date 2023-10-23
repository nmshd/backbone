using Backbone.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Relationships.Application.Relationships.Commands.AcceptRelationshipChangeRequest;

public class AcceptRelationshipChangeRequestCommand : IRequest<AcceptRelationshipChangeRequestResponse>
{
    public RelationshipId Id { get; set; }
    public RelationshipChangeId ChangeId { get; set; }
    public byte[] ResponseContent { get; set; }
}
