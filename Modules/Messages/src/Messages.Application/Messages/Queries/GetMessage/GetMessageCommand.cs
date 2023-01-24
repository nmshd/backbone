using MediatR;
using Messages.Application.Messages.DTOs;
using Messages.Domain.Ids;

namespace Messages.Application.Messages.Queries.GetMessage;

public class GetMessageCommand : IRequest<MessageDTO>
{
    public MessageId Id { get; init; }
    public bool NoBody { get; init; }
}
