namespace Backbone.AdminApi.Tests.Integration.Models;

public class MessageOverviewDTO
{
    public string MessageId { get; set; } = null!;
    public string SenderAddress { get; set; } = null!;
    public string SenderDevice { get; set; } = null!;
    public DateTime SendDate { get; set; }
    public int NumberOfAttachments { get; set; }
    public List<MessageRecipientDTO> Recipients { get; set; } = null!;
}
