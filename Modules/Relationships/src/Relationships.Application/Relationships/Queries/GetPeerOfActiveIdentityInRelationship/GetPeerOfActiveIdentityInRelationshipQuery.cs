using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetPeerOfActiveIdentityInRelationship;

public class GetPeerOfActiveIdentityInRelationshipQuery : IRequest<GetPeerOfActiveIdentityInRelationshipResponse>
{
    public required string Id { get; init; }
}
