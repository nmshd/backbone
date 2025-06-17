using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStatusChanged;

public class IdentityDeletionProcessStatusChangedDomainEvent : DomainEvent
{
    public required string DeletionProcessOwner { get; set; }
    public required string DeletionProcessId { get; set; }
    public string? Initiator { get; set; }
}
