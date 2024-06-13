using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetPeerOfActiveIdentityInRelationship;

public class GetPeerOfActiveIdentityInRelationshipQuery : IRequest<IdentityAddress>
{
    public required RelationshipId Id { get; set; }
}
