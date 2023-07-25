using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipChangeCompleted;
public class RelationshipChangeCompletedIntegrationEvent : IntegrationEvent
{
    public string ChangeId { get; set; }
    public string RelationshipId { get; set; }
    public string ChangeCreatedBy { get; set; }
    public string ChangeRecipient { get; set; }
    public string ChangeResult { get; set; }
}
