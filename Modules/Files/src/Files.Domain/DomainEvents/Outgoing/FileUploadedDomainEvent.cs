using Backbone.BuildingBlocks.Domain.Events;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Domain.DomainEvents.Outgoing;

public class FileUploadedDomainEvent : DomainEvent
{
    public FileUploadedDomainEvent(File file) : base($"{file.Id}/Created")
    {
        Uploader = file.CreatedBy.ToString();
        FileId = file.Id.ToString();
    }

    public string FileId { get; private set; }
    public string Uploader { get; private set; }
}
