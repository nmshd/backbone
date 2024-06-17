using Backbone.Modules.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetPeerOfActiveIdentityInRelationship;

public class GetPeerOfActiveIdentityInRelationshipQuery : IRequest<GetPeerOfActiveIdentityInRelationshipResponse>
{
    public required RelationshipId Id { get; set; }
}
