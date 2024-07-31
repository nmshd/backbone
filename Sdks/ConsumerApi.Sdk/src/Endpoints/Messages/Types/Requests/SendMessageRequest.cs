namespace Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;

public class SendMessageRequest
{
    public required List<SendMessageRequestRecipientInformation> Recipients { get; set; }
    public required byte[] Body { get; set; }
    public required List<Attachment> Attachments { get; set; }
}

public class SendMessageRequestRecipientInformation
{
    public required string Address { get; set; }
    public required byte[] EncryptedKey { get; set; }
}
