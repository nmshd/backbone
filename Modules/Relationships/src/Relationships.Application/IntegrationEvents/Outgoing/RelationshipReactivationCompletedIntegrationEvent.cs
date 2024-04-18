using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
public class RelationshipReactivationCompletedIntegrationEvent : IntegrationEvent
{
    public RelationshipReactivationCompletedIntegrationEvent(Relationship relationship, IdentityAddress peer)
        : base($"{relationship.Id}/ReactivationCompleted/{relationship.AuditLog.Last().CreatedAt}")
    {
        RelationshipId = relationship;
        Peer = peer;
    }

    public Relationship RelationshipId { get; }
    public IdentityAddress Peer { get; set; }
}
