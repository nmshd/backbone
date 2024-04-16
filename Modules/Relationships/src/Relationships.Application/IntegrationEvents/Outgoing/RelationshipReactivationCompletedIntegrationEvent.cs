using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
public class RelationshipReactivationCompletedIntegrationEvent : IntegrationEvent
{
    public RelationshipReactivationCompletedIntegrationEvent(Relationship relationship, IdentityAddress partner) :
        base($"{relationship.Id}/Reactivation/Revoked/{relationship.AuditLog.Last().CreatedAt}")
    {
        RelationshipId = relationship.Id;
        CreatedAt = relationship.AuditLog.Last().CreatedAt;
        Partner = partner.StringValue;
    }

    public string RelationshipId { get; }
    public DateTime CreatedAt { get; }
    public string Partner { get; }
}
