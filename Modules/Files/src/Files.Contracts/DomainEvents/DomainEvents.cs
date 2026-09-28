using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Files.Contracts.DomainEvents;

public class FileOwnershipClaimedDomainEvent : DomainEvent
{
    public required string FileId { get; set; }
    public required string OldOwnerAddress { get; set; }
    public required string NewOwnerAddress { get; set; }
}

public class FileOwnershipLockedDomainEvent : DomainEvent
{
    public required string FileId { get; set; }
    public required string OwnerAddress { get; set; }
}

public class FileUploadedDomainEvent : DomainEvent
{
    public required string FileId { get; set; }
    public required string Owner { get; set; }
}
