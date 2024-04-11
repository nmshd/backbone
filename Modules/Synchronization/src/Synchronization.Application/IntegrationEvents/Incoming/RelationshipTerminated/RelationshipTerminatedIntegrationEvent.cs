using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipTerminated;
public class RelationshipTerminatedIntegrationEvent : IntegrationEvent
{
    public RelationshipTerminatedIntegrationEvent(string relationshipId, DateTime createdAt, string partner) : 
        base($"{relationshipId}/Terminated/{createdAt}")
    {
        RelationshipId = relationshipId;
        CreatedAt = createdAt;
        Partner = partner;
    }

    public string RelationshipId { get; }
    public DateTime CreatedAt { get; set; }
    public string Partner { get; }
}
