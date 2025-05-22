namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Messages;

public class Message
{
    public string Id { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public List<MessageRecipient> Recipients { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public byte[] Body { get; set; } = null!;
}

public class MessageRecipient
{
    public string Id { get; set; } = null!;
    public string MessageId { get; set; } = null!;
    public string RelationshipId { get; set; } = null!;
    public string Address { get; set; } = null!;
    public DateTime? ReceivedAt { get; set; }
}
