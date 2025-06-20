using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationship;

public class RejectRelationshipCommand : IRequest<RejectRelationshipResponse>
{
    public required string RelationshipId { get; init; }
    public byte[]? CreationResponseContent { get; init; }
}
