using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipReactivationCompleted;
public class RelationshipReactivationCompletedIntegrationEvent : IntegrationEvent
{
    public RelationshipReactivationCompletedIntegrationEvent(string relationshipId, DateTime createdAt, string peer) :

        base($"{relationshipId}/Reactivation/Revoked/{createdAt}")
    {
        RelationshipId = relationshipId;
        CreatedAt = createdAt;
        Peer = peer;
    }

    public string RelationshipId { get; }
    public DateTime CreatedAt { get; set; }
    public string Peer { get; }
}
