using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Domain.DomainEvents.Outgoing;

public class FileOwnershipClaimedDomainEvent : DomainEvent
{
    public FileOwnershipClaimedDomainEvent(File file, IdentityAddress oldOwnerAddress)
    {
        FileId = file.Id.Value;
        OldOwnerAddress = oldOwnerAddress.Value;
        NewOwnerAddress = file.Owner.Value;
    }

    public string FileId { get; set; }
    public string OldOwnerAddress { get; set; }
    public string NewOwnerAddress { get; set; }
}
