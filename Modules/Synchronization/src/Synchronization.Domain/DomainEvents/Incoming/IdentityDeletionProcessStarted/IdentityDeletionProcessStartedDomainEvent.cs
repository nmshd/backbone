using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStarted;

public class IdentityDeletionProcessStartedDomainEvent : DomainEvent
{
    public IdentityDeletionProcessStartedDomainEvent(string address, string deletionProcessId, string? initiator)
    {
        DeletionProcessId = deletionProcessId;
        Address = address;
        Initiator = initiator;
    }

    public string Address { get; private set; }
    public string DeletionProcessId { get; private set; }
    public string? Initiator { get; }
}
