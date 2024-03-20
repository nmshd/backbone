using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RevokeRelationshipChangeRequest;

public class RevokeRelationshipChangeRequestCommand : IRequest<RevokeRelationshipChangeRequestResponse>
{
    public required RelationshipId Id { get; set; }
    public required RelationshipChangeId ChangeId { get; set; }
    public byte[]? ResponseContent { get; set; }
}
