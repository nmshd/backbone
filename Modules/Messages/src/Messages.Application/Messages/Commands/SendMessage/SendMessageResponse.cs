using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Application.Messages.Commands.SendMessage;

public class SendMessageResponse
{
    public SendMessageResponse(Message message)
    {
        Id = message.Id;
        CreatedAt = message.CreatedAt;
    }

    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
