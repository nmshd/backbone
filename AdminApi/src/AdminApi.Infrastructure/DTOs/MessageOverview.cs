namespace Backbone.AdminApi.Infrastructure.DTOs;
public class MessageOverview
{
    public string MessageId { get; set; }
    public string SenderAddress { get; set; }
    public string SenderDevice { get; set; }
    public DateTime SendDate { get; set; }
    public int NumberOfAttachments { get; set; }
    public List<MessageRecipient> Recipients { get; set; }
}
