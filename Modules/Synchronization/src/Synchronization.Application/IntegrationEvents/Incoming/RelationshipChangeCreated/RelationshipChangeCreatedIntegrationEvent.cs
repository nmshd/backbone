using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipChangeCreated;

public class RelationshipChangeCreatedIntegrationEvent : IntegrationEvent
{
    public string ChangeId { get; set; }
    public string RelationshipId { get; set; }
    public string ChangeCreatedBy { get; set; }
    public string ChangeRecipient { get; set; }
}
