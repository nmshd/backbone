using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStatusChanged;

public class IdentityDeletionProcessStatusChangedDomainEvent : DomainEvent
{
    public IdentityDeletionProcessStatusChangedDomainEvent(string address, string deletionProcessId, string? initiator)
    {
        Address = address;
        DeletionProcessId = deletionProcessId;
        Initiator = initiator;
    }

    public string Address { get; }
    public string DeletionProcessId { get; }
    public string? Initiator { get; }
}
