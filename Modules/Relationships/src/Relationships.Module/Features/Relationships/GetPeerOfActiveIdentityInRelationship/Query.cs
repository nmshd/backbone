using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.GetPeerOfActiveIdentityInRelationship;

public class GetPeerOfActiveIdentityInRelationshipQuery : IRequest<GetPeerOfActiveIdentityInRelationshipResponse>
{
    public required string Id { get; init; }
}
