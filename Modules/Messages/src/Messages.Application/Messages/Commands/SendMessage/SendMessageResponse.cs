using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Messages.Domain.Entities;
using Backbone.Messages.Domain.Ids;

namespace Backbone.Messages.Application.Messages.Commands.SendMessage;

public class SendMessageResponse : IMapTo<Message>
{
    public MessageId Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
