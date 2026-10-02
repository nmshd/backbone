using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RevokeRelationship;

public class Command : IRequest<Response>
{
    public required string RelationshipId { get; init; }
    public byte[]? CreationResponseContent { get; init; }
}
