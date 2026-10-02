using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RevokeRelationship;

public class RevokeRelationshipCommand : IRequest<RevokeRelationshipResponse>
{
    public required string RelationshipId { get; init; }
    public byte[]? CreationResponseContent { get; init; }
}
