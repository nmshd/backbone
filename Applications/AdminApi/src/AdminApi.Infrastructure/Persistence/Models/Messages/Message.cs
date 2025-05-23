namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Messages;

public class Message
{
    public string Id { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public string CreatedByDevice { get; set; } = null!;
    public List<MessageRecipient> Recipients { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public byte[] Body { get; set; } = null!;
    public IEnumerable<MessageAttachment> Attachments { get; set; } = null!;
}

public class MessageAttachment
{
    public string Id { get; set; } = null!;
    public string MessageId { get; set; } = null!;
}

public class MessageRecipient
{
    public int Id { get; set; }

    public string MessageId { get; set; } = null!;
    public string RelationshipId { get; set; } = null!;
    public string Address { get; set; } = null!;
    public DateTime? ReceivedAt { get; set; }
}
