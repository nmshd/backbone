using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.FileUploaded;

public class FileUploadedDomainEvent : DomainEvent
{
    public required string FileId { get; set; }
    public required string Owner { get; set; }
}
