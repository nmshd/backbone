using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;

public class RelationshipCreatedIntegrationEvent : IntegrationEvent
{
    public RelationshipCreatedIntegrationEvent(Relationship relationship) : base($"{relationship.Id}/Created")
    {
        RelationshipId = relationship.Id;
        From = relationship.From;
        To = relationship.To;
    }

    public string RelationshipId { get; set; }
    public string From { get; }
    public string To { get; }
}
