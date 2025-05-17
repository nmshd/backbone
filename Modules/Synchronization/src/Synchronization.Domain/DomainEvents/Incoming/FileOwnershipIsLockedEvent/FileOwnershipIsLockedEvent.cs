using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.FileOwnershipIsLockedEvent;

public class FileOwnershipIsLockedEvent : DomainEvent
{
    public string FileAddress { get; set; }
    public string OwnerAddress { get; set; }

    public FileOwnershipIsLockedEvent(string fileAddress, string ownerAddress)
    {
        FileAddress = fileAddress;
        OwnerAddress = ownerAddress;
    }
}
