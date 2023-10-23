using Backbone.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Relationships.Application.Relationships.Commands.RevokeRelationshipChangeRequest;

public class RevokeRelationshipChangeRequestCommand : IRequest<RevokeRelationshipChangeRequestResponse>
{
    public RelationshipId Id { get; set; }
    public RelationshipChangeId ChangeId { get; set; }
    public byte[] ResponseContent { get; set; }
}
