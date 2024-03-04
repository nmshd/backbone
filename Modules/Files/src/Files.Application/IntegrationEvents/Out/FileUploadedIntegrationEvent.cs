using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.IntegrationEvents.Out;
public class FileUploadedIntegrationEvent : IntegrationEvent
{
    public FileUploadedIntegrationEvent(File file) : base($"{file.Id}/Created")
    {
        Uploader = file.CreatedBy.ToString();
        FileId = file.Id.ToString();
    }

    public string FileId { get; private set; }
    public string Uploader { get; private set; }
}
