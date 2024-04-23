using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class IdentityDeletionProcessStatusChangedDomainEvent : DomainEvent
{
    public IdentityDeletionProcessStatusChangedDomainEvent(string identityAddress, string deletionProcessId)
        : base($"{identityAddress}/IdentityDeletionProcessStatusChanged/{deletionProcessId}")
    {
        Address = identityAddress;
        DeletionProcessId = deletionProcessId;
    }

    public string Address { get; }
    public string DeletionProcessId { get; }
}
