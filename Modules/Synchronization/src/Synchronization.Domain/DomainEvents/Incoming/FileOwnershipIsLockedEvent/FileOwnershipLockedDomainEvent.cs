using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.FileOwnershipIsLockedEvent;

public class FileOwnershipLockedDomainEvent : DomainEvent
{
    public required string FileId { get; set; }
    public required string OwnerAddress { get; set; }
}
