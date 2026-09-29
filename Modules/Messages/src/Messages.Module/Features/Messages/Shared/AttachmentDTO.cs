using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Module.Features.Messages.Shared;

public class AttachmentDTO(Attachment attachment)
{
    public string Id { get; set; } = attachment.Id;
}
