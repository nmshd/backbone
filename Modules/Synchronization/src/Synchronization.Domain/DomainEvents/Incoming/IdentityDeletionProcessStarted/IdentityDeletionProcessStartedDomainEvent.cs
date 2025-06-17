using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStarted;

public class IdentityDeletionProcessStartedDomainEvent : DomainEvent
{
    public required string Address { get; set; }
    public required string DeletionProcessId { get; set; }
    public string? Initiator { get; set; }
}
