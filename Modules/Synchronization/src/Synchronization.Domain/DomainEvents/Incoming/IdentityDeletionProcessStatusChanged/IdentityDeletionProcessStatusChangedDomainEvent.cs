using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStatusChanged;

public class IdentityDeletionProcessStatusChangedDomainEvent : DomainEvent
{
    public IdentityDeletionProcessStatusChangedDomainEvent(string address, string deletionProcessId)
    {
        Address = address;
        DeletionProcessId = deletionProcessId;
    }

    public string Address { get; }
    public string DeletionProcessId { get; }
}
