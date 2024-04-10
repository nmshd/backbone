using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipReactivationRevoked;
public class RelationshipReactivationRevokedIntegrationEvent : IntegrationEvent
{
    public RelationshipReactivationRevokedIntegrationEvent(string relationshipId, string partner) : base($"{relationshipId}/Reactivation/Revoked")
    {
        RelationshipId = relationshipId;
        Partner = partner;
    }

    public string RelationshipId { get; }
    public string Partner { get; }
}
