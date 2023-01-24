using MediatR;
using Relationships.Domain.Ids;

namespace Relationships.Application.Relationships.Commands.RevokeRelationshipChangeRequest;

public class RevokeRelationshipChangeRequestCommand : IRequest<RevokeRelationshipChangeRequestResponse>
{
    public RelationshipId Id { get; set; }
    public RelationshipChangeId ChangeId { get; set; }
    public byte[] ResponseContent { get; set; }
}
