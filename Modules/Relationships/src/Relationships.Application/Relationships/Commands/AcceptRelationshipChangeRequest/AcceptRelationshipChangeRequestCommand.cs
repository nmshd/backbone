using MediatR;
using Relationships.Domain.Ids;

namespace Relationships.Application.Relationships.Commands.AcceptRelationshipChangeRequest;

public class AcceptRelationshipChangeRequestCommand : IRequest<AcceptRelationshipChangeRequestResponse>
{
    public RelationshipId Id { get; set; }
    public RelationshipChangeId ChangeId { get; set; }
    public byte[] ResponseContent { get; set; }
}
