using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Messages.Domain.Entities;
using Messages.Domain.Ids;

namespace Messages.Application.Messages.Commands.SendMessage;

public class SendMessageResponse : IMapTo<Message>
{
    public MessageId Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
