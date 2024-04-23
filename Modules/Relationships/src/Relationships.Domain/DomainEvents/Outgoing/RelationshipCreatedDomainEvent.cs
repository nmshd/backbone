using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Outgoing;

public class RelationshipCreatedDomainEvent : DomainEvent
{
    public RelationshipCreatedDomainEvent(Relationship relationship) : base($"{relationship.Id}/Created")
    {
        RelationshipId = relationship.Id;
        From = relationship.From;
        To = relationship.To;
    }

    public string RelationshipId { get; set; }
    public string From { get; }
    public string To { get; }
}
