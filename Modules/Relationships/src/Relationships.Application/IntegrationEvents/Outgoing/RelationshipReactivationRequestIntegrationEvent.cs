using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
public class RelationshipReactivationRequestIntegrationEvent : IntegrationEvent
{
    public RelationshipReactivationRequestIntegrationEvent(Relationship relationship, IdentityAddress peer) : base($"{relationship.Id}/Reactivate")
    {
        RelationshipId = relationship.Id;
        Peer = peer.StringValue;
    }

    public string RelationshipId { get; }
    public string Peer { get; }
}
