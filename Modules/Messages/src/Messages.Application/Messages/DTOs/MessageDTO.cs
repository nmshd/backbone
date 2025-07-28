using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Application.Messages.DTOs;

public class MessageDTO
{
    public MessageDTO(Message message, IdentityAddress activeIdentity, string didDomainName)
    {
        Id = message.Id;
        CreatedAt = message.CreatedAt;
        CreatedBy = message.CreatedBy;
        CreatedByDevice = message.CreatedByDevice;
        Body = message.Details.Body;
        Attachments = message.Attachments.Select(a => new AttachmentDTO(a)).ToList();
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
            // recipients should only see themselves in the list of recipients
            if (message.CreatedBy != activeIdentity && recipient.Address != activeIdentity)
                continue;

            var recipientInformationDTO = new RecipientInformationDTO(recipient);
            recipients.Add(recipientInformationDTO);

            // a sender who has decomposed the relationship to a recipient should see the recipient as anonymized
            if (message.CreatedBy == activeIdentity && recipient.IsRelationshipDecomposedBySender)
                recipientInformationDTO.Address = IdentityAddress.GetAnonymized(didDomainName);
        }

        return recipients;
    }
}
