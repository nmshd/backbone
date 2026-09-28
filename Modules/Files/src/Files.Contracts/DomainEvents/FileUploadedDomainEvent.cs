using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Files.Contracts.DomainEvents;

public class FileUploadedDomainEvent : DomainEvent
{
    public required string FileId { get; set; }
    public required string Owner { get; set; }
}
