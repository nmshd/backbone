using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class IdentityDeletionProcessStatusChangedDomainEvent : DomainEvent
{
    public IdentityDeletionProcessStatusChangedDomainEvent(string identityAddress, string deletionProcessId, string? initiator)
        : base($"{identityAddress}/IdentityDeletionProcessStatusChanged/{deletionProcessId}")
    {
        Address = identityAddress;
        DeletionProcessId = deletionProcessId;
        Initiator = initiator;
    }

    public string Address { get; }
    public string DeletionProcessId { get; }
    public string? Initiator { get; }
}
