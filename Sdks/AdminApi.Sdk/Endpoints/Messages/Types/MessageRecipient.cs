namespace Backbone.AdminApi.Sdk.Endpoints.Messages.Types;

public class MessageRecipient
{
    public int Id { get; set; }
    public string Address { get; set; } = null!;
    public string MessageId { get; set; } = null!;
}
