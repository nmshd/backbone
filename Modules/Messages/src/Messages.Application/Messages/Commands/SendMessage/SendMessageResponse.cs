using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Backbone.Modules.Messages.Application.Messages.Commands.SendMessage;

public class SendMessageResponse : IMapTo<Message>
{
    public MessageId Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
