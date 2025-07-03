using Backbone.BuildingBlocks.Domain.Events;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Domain.DomainEvents.Outgoing;

public class FileUploadedDomainEvent : DomainEvent
{
    public FileUploadedDomainEvent(File file) : base($"{file.Id}/Created")
    {
        Owner = file.Owner.ToString();
        FileId = file.Id.ToString();
    }

    public string FileId { get; init; }
    public string Owner { get; init; }
}
