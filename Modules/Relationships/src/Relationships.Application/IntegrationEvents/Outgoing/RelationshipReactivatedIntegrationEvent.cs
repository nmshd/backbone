using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
public class RelationshipReactivatedIntegrationEvent : IntegrationEvent
{
    public RelationshipReactivatedIntegrationEvent(Relationship relationship, IdentityAddress partner) : base($"{relationship.Id}/Reactivate")
    {
        RelationshipId = relationship.Id;
        Partner = partner.StringValue;
    }

    public string RelationshipId { get; }
    public string Partner { get; }
}
