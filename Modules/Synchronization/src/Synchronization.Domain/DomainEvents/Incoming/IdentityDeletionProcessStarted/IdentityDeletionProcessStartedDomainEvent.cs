using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStarted;

public class IdentityDeletionProcessStartedDomainEvent : DomainEvent
{
    public string Address { get; set; } = null!;
    public string DeletionProcessId { get; set; } = null!;
    public string? Initiator { get; set; }
}
