using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipChangeCompleted;

public class RelationshipChangeCompletedIntegrationEvent : IntegrationEvent
{
    public required string ChangeId { get; set; }
    public required string RelationshipId { get; set; }
    public required string ChangeCreatedBy { get; set; }
    public required string ChangeRecipient { get; set; }
    public required string ChangeResult { get; set; }
}
