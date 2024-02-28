namespace Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types;

public class SendMessageRequest
{
    public List<SendMessageRequestRecipientInformation> Recipients { get; set; } = [];
    public DateTime? DoNotSendBefore { get; set; }
    public required byte[] Body { get; set; }
    public List<Attachment> Attachments { get; set; } = [];
}

public class SendMessageRequestRecipientInformation
{
    public required string Address { get; set; }
    public required byte[] EncryptedKey { get; set; }
}
