using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RejectRelationship;

public class RejectRelationshipCommand : IRequest<RejectRelationshipResponse>
{
    public required string RelationshipId { get; init; }
    public byte[]? CreationResponseContent { get; init; }
}
