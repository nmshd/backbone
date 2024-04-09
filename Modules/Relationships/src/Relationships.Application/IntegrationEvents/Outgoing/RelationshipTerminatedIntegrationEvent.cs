using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;
public class RelationshipTerminatedIntegrationEvent : IntegrationEvent
{
    public RelationshipTerminatedIntegrationEvent(Relationship relationship, IdentityAddress partner) : base($"{relationship.Id}/Terminated")
    {
        RelationshipId = relationship.Id;
        Partner = partner.StringValue;
    }

    public string RelationshipId { get; }
    public string Partner { get; }
}
