using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Application.Messages.DTOs;

public class MessageDTO : IMapTo<Message>
{
    public required MessageId Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public required IdentityAddress CreatedBy { get; set; }
    public required DeviceId CreatedByDevice { get; set; }

    public DateTime? DoNotSendBefore { get; set; }
    public required byte[] Body { get; set; }

    public required List<AttachmentDTO> Attachments { get; set; }
    public List<RecipientInformationDTO>? Recipients { get; set; }

    public void PrepareForActiveIdentity(IdentityAddress activeIdentity)
    {
        if (CreatedBy != activeIdentity && Recipients != null)
            Recipients.RemoveAll(r => r.Address != activeIdentity);
    }
}
