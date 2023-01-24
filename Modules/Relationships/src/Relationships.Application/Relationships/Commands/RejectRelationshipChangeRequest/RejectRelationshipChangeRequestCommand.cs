using MediatR;
using Relationships.Domain.Ids;

namespace Relationships.Application.Relationships.Commands.RejectRelationshipChangeRequest;

public class RejectRelationshipChangeRequestCommand : IRequest<RejectRelationshipChangeRequestResponse>
{
    public RelationshipId Id { get; set; }
    public RelationshipChangeId ChangeId { get; set; }
    public byte[] ResponseContent { get; set; }
}
