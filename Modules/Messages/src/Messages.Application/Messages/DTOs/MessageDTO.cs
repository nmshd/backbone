using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Messages.Domain.Entities;
using Backbone.Messages.Domain.Ids;

namespace Backbone.Messages.Application.Messages.DTOs;

public class MessageDTO : IMapTo<Message>
{
    public MessageId Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }

    public DateTime? DoNotSendBefore { get; set; }
    public byte[] Body { get; set; }

    public List<AttachmentDTO> Attachments { get; set; }
    public List<RecipientInformationDTO> Recipients { get; set; }

    public void PrepareForActiveIdentity(IdentityAddress activeIdentity)
    {
        if (CreatedBy != activeIdentity)
            Recipients.RemoveAll(r => r.Address != activeIdentity);
    }
}
