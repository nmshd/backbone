using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
public class RelationshipTerminatedIntegrationEvent : IntegrationEvent
{
    public RelationshipTerminatedIntegrationEvent(Relationship relationship, IdentityAddress peer) :
        base($"{relationship.Id}/Terminated/{relationship.AuditLog.Last().CreatedAt}")
    {
        RelationshipId = relationship.Id;
        Peer = peer.StringValue;
        CreatedAt = relationship.AuditLog.Last().CreatedAt;
    }

    public string RelationshipId { get; }
    public DateTime CreatedAt;
    public string Peer { get; }
}
