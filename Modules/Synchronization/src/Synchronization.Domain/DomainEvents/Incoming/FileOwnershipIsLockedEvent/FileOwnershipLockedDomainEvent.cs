using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.FileOwnershipIsLockedEvent;

public class FileOwnershipLockedDomainEvent : DomainEvent
{
    public string FileId { get; set; }
    public string OwnerAddress { get; set; }

    public FileOwnershipLockedDomainEvent(string fileId, string ownerAddress)
    {
        FileId = fileId;
        OwnerAddress = ownerAddress;
    }
}
