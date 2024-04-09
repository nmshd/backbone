using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipTerminated;
public class RelationshipTerminatedIntegrationEvent : IntegrationEvent
{
    public RelationshipTerminatedIntegrationEvent(string relationshipId, string partner) : base($"{relationshipId}/Terminated")
    {
        RelationshipId = relationshipId;
        Partner = partner;
    }

    public string RelationshipId { get; }
    public string Partner { get; }
}
