using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.Relationships.Domain.Entities;

namespace Backbone.Relationships.Application.IntegrationEvents;
public class RelationshipChangeCreatedIntegrationEvent : IntegrationEvent
{
    public RelationshipChangeCreatedIntegrationEvent(RelationshipChange change) : base($"{change.Id}/Created")
    {
        ChangeId = change.Id;
        RelationshipId = change.RelationshipId;
        ChangeCreatedBy = change.Request.CreatedBy;
        ChangeRecipient = change.Request.CreatedBy == change.Relationship.From ? change.Relationship.To : change.Relationship.From;
    }

    public string ChangeId { get; }
    public string RelationshipId { get; }
    public string ChangeCreatedBy { get; }
    public string ChangeRecipient { get; }
}
