using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Application.Messages.DTOs;

public class RecipientInformationDTO
{
    public RecipientInformationDTO(RecipientInformation recipientInformation)
    {
        Address = recipientInformation.Address;
        EncryptedKey = recipientInformation.EncryptedKey;
        ReceivedAt = recipientInformation.ReceivedAt;
        ReceivedByDevice = recipientInformation.ReceivedByDevice?.Value;
    }

    public string Address { get; set; }
    public byte[] EncryptedKey { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public string? ReceivedByDevice { get; set; }
}
