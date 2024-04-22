using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
public class RelationshipDecomposedIntegrationEvent : IntegrationEvent
{
    public RelationshipDecomposedIntegrationEvent(Relationship relationship, IdentityAddress peer) : base($"{relationship.Id}/Decompose/{relationship.AuditLog.Last().CreatedAt}")
    {
        RelationshipId = relationship.Id;
        Peer = peer.StringValue;
    }

    public string RelationshipId { get; }
    public string Peer { get; }
}
