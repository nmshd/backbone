using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Module.Features.Messages.Shared;

public class MessageDTO
{
    public MessageDTO(Message message, IdentityAddress activeIdentity, string didDomainName)
    {
        Id = message.Id;
        CreatedAt = message.CreatedAt;
        CreatedBy = message.CreatedBy;
        CreatedByDevice = message.CreatedByDevice;
        Body = message.Details.Body;
        Attachments = message.Attachments.Select(attachment => new AttachmentDTO(attachment)).ToList();
        Recipients = MapRecipients(message, activeIdentity, didDomainName);
    }

    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string CreatedByDevice { get; set; }
    public byte[] Body { get; set; }
    public List<AttachmentDTO> Attachments { get; set; }
    public List<RecipientInformationDTO> Recipients { get; set; }

    private static List<RecipientInformationDTO> MapRecipients(Message message, IdentityAddress activeIdentity, string didDomainName)
    {
        List<RecipientInformationDTO> recipients = [];
        foreach (var recipient in message.Recipients)
        {
            if (message.CreatedBy != activeIdentity && recipient.Address != activeIdentity)
                continue;

            var dto = new RecipientInformationDTO(recipient);
            recipients.Add(dto);
            if (message.CreatedBy == activeIdentity && recipient.IsRelationshipDecomposedBySender)
                dto.Address = IdentityAddress.GetAnonymized(didDomainName);
        }

        return recipients;
    }
}
