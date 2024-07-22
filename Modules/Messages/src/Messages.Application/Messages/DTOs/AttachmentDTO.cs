using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Application.Messages.DTOs;

public class AttachmentDTO
{
    public AttachmentDTO(Attachment attachment)
    {
        Id = attachment.Id;
    }

    public string Id { get; set; }
}
