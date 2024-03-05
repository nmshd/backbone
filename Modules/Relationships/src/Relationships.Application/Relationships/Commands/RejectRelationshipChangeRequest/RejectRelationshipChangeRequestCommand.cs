using Backbone.Modules.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationshipChangeRequest;

public class RejectRelationshipChangeRequestCommand : IRequest<RejectRelationshipChangeRequestResponse>
{
    public required RelationshipId Id { get; set; }
    public required RelationshipChangeId ChangeId { get; set; }
    public byte[]? ResponseContent { get; set; }
}
