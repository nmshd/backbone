using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStatusChanged;

public class IdentityDeletionProcessStatusChangedIntegrationEvent : IntegrationEvent
{
    public IdentityDeletionProcessStatusChangedIntegrationEvent(string address, string deletionProcessId)
    {
        Address = address;
        DeletionProcessId = deletionProcessId;
    }

    public string Address { get; }
    public string DeletionProcessId { get; }
}
