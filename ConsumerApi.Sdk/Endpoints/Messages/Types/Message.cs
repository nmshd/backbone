namespace Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types;

public class Message
{
    public required string Id { get; set; }

    public required DateTime CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    public required string CreatedByDevice { get; set; }

    public DateTime? DoNotSendBefore { get; set; }
    public required byte[] Body { get; set; }

    public required List<Attachment> Attachments { get; set; }
    public required List<RecipientInformation> Recipients { get; set; }
}

public class Attachment
{
    public required string Id { get; set; }
}

public class RecipientInformation
{
    public required string Address { get; set; }
    public required byte[] EncryptedKey { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public string? ReceivedByDevice { get; set; }
}
