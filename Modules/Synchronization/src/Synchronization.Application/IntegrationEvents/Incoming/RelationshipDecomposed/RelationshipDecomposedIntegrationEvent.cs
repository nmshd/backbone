using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipDecomposed;
public class RelationshipDecomposedIntegrationEvent : DomainEvent
{
    public RelationshipDecomposedIntegrationEvent(string relationshipId, string peer) : base($"{relationshipId}/Decompose")
    {
        RelationshipId = relationshipId;
        Peer = peer;
    }

    public string RelationshipId { get; }
    public string Peer { get; }
}
