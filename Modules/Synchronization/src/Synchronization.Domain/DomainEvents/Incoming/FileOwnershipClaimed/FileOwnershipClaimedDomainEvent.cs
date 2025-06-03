using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.FileOwnershipClaimed;

public class FileOwnershipClaimedDomainEvent : DomainEvent
{
    public required string FileId { get; set; }
    public required string OldOwnerAddress { get; set; }
    public required string NewOwnerAddress { get; set; }
}
