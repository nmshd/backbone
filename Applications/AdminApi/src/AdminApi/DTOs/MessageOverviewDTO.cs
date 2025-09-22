using Backbone.AdminApi.Infrastructure.Persistence.Models.Messages;

namespace Backbone.AdminApi.DTOs;

public class MessageOverviewDTO
{
    public string MessageId { get; set; } = null!;
    public string SenderAddress { get; set; } = null!;
    public string SenderDevice { get; set; } = null!;
    public DateTime SendDate { get; set; }
    public int NumberOfAttachments { get; set; }
    public List<MessageRecipient> Recipients { get; set; } = null!;
}
