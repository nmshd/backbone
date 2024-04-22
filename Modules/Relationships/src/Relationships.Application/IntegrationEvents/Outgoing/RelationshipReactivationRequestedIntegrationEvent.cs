using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
public class RelationshipReactivationRequestedIntegrationEvent : IntegrationEvent
{
    public RelationshipReactivationRequestedIntegrationEvent(Relationship relationship, IdentityAddress requestingIdentity, IdentityAddress peer) : 
        base($"{relationship.Id}/ReactivationRequested/{relationship.AuditLog.Last().CreatedAt}")
    {
        RelationshipId = relationship.Id;
        RequestingIdentity = requestingIdentity.StringValue;
        Peer = peer.StringValue;
    }


    public string RelationshipId { get; } // todo: this may be unnecessary
    public string RequestingIdentity { get; set; } // todo: check if there is a better name for this property
    public string Peer { get; } // todo: this may be unnecessary
}
