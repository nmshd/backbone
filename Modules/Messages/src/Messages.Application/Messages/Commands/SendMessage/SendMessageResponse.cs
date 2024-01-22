using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Application.Messages.Commands.SendMessage;

public class SendMessageResponse : IMapTo<Message>
{
    public MessageId? Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
