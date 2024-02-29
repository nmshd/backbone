using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStatusChanged;
public class IdentityDeletionProcessStatusChangedIntegrationEvent : IntegrationEvent
{
    public IdentityDeletionProcessStatusChangedIntegrationEvent(string identityAddress, string deletionProcessId)
        : base($"{identityAddress}/DeletionProcessStarted/{deletionProcessId}") // unsure about if this is needed and what for ?
    {
        Address = identityAddress;
        DeletionProcessId = deletionProcessId;
    }

    public string Address { get; internal set; }
    public object DeletionProcessId { get; internal set; }
}
