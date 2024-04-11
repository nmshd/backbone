using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipReactivationRevoked;
public class RelationshipReactivationRevokedIntegrationEvent : IntegrationEvent
{
    public RelationshipReactivationRevokedIntegrationEvent(string relationshipId, DateTime createdAt, string partner) :

        base($"{relationshipId}/Reactivation/Revoked/{createdAt}")
    {
        RelationshipId = relationshipId;
        CreatedAt = createdAt;
        Partner = partner;
    }

    public string RelationshipId { get; }
    public DateTime CreatedAt { get; set; }
    public string Partner { get; }
}
