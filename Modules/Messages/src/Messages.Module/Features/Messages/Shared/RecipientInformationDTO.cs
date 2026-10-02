using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Module.Features.Messages.Shared;

public class RecipientInformationDTO(RecipientInformation recipientInformation)
{
    public string Address { get; set; } = recipientInformation.Address;
    public byte[] EncryptedKey { get; set; } = recipientInformation.EncryptedKey;
    public DateTime? ReceivedAt { get; set; } = recipientInformation.ReceivedAt;
    public string? ReceivedByDevice { get; set; } = recipientInformation.ReceivedByDevice?.Value;
}
