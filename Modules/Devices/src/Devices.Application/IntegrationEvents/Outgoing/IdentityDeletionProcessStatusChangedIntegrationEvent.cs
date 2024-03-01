using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
public class IdentityDeletionProcessStatusChangedIntegrationEvent : IntegrationEvent
{
    public IdentityDeletionProcessStatusChangedIntegrationEvent(string identityAddress, string deletionProcessId)
        : base($"{identityAddress}/IdentityDeletionProcessStatusChanged/{deletionProcessId}")
    {
        Address = identityAddress;
        DeletionProcessId = deletionProcessId;
    }

    public string Address { get; }
    public object DeletionProcessId { get; }
}
