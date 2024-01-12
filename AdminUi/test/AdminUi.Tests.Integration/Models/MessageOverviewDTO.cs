namespace Backbone.AdminUi.Tests.Integration.Models;
public class MessageOverviewDTO
{
    public string MessageId { get; set; }
    public string SenderAddress { get; set; }
    public string SenderDevice { get; set; }
    public DateTime SendDate { get; set; }
    public int NumberOfAttachments { get; set; }
    public List<MessageRecipientDTO> Recipients { get; set; }
}
