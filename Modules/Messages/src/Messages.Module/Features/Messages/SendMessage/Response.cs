using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Module.Features.Messages.SendMessage;

public class Response
{
    public Response(Message message)
    {
        Id = message.Id;
        CreatedAt = message.CreatedAt;
    }

    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
