using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipReactivationCompleted;
internal class RelationshipReactivationCompletedIntegrationEvent : IntegrationEvent
{
    public RelationshipReactivationCompletedIntegrationEvent(string relationshipId, string peer) : base($"{relationshipId}/RelationshipReactivation/Completed")
    {
        RelationshipId = relationshipId;
        Peer = peer;
    }

    public string RelationshipId { get; }
    public string Peer { get; set; }
}
