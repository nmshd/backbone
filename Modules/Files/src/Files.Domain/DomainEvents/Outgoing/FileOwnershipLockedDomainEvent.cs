using Backbone.BuildingBlocks.Domain.Events;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Domain.DomainEvents.Outgoing;

public class FileOwnershipLockedDomainEvent : DomainEvent
{
    public string FileId { get; set; }
    public string OwnerAddress { get; set; }

    public FileOwnershipLockedDomainEvent(File file)
    {
        FileId = file.Id.Value;
        OwnerAddress = file.Owner.Value;
    }
}
