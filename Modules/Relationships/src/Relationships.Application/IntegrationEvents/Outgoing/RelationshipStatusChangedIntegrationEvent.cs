using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;

public class RelationshipStatusChangedIntegrationEvent : IntegrationEvent
{
    public RelationshipStatusChangedIntegrationEvent(Relationship relationship) : base($"{relationship.Id}/StatusChanged/{relationship.AuditLog.Last().CreatedAt.ToUniversalString()}")
    {
        RelationshipId = relationship.Id;
        Status = relationship.Status.ToDtoString();
        From = relationship.From;
        To = relationship.To;
    }

    public string RelationshipId { get; set; }
    public string Status { get; set; }
    public string From { get; set; }
    public string To { get; set; }
}
