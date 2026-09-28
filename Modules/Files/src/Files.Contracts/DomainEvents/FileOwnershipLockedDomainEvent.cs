using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Files.Contracts.DomainEvents;

public class FileOwnershipLockedDomainEvent : DomainEvent
{
    public required string FileId { get; set; }
    public required string OwnerAddress { get; set; }
}
