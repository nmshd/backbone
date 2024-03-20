using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationshipChangeRequest;

public class AcceptRelationshipChangeRequestCommand : IRequest<AcceptRelationshipChangeRequestResponse>
{
    public required RelationshipId Id { get; set; }
    public required RelationshipChangeId ChangeId { get; set; }
    public byte[]? ResponseContent { get; set; }
}
