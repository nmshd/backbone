using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipReactivated;
public class RelationshipReactivatedIntegrationEvent : IntegrationEvent
{
    public RelationshipReactivatedIntegrationEvent(string relationshipId, string partner) : base($"{relationshipId}/Reactivate")
    {
        RelationshipId = relationshipId;
        Partner = partner;
    }

    public string RelationshipId { get; }
    public string Partner { get; }
}
