namespace Backbone.AdminUi.Tests.Integration.Models;

public class MessagesDownloadDTO
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public MessageBody Body { get; set; }
}

public class MessageBody
{
    public string Type { get; set; }
    public string To { get; set; }
    public string Content { get; set; }
}
