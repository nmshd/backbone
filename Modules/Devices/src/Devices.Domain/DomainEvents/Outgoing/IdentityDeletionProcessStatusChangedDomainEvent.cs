using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

public class IdentityDeletionProcessStatusChangedDomainEvent : DomainEvent
{
    public IdentityDeletionProcessStatusChangedDomainEvent(string deletionProcessOwner, string deletionProcessId, string? initiator)
        : base($"{deletionProcessOwner}/IdentityDeletionProcessStatusChanged/{deletionProcessId}")
    {
        DeletionProcessOwner = deletionProcessOwner;
        DeletionProcessId = deletionProcessId;
        Initiator = initiator;
    }

    public string DeletionProcessOwner { get; }
    public string DeletionProcessId { get; }
    public string? Initiator { get; }
}
