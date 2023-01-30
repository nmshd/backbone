using Backbone.Modules.Messages.Application.Messages.DTOs;
using Backbone.Modules.Messages.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Queries.GetMessage;

public class GetMessageCommand : IRequest<MessageDTO>
{
    public MessageId Id { get; init; }
    public bool NoBody { get; init; }
}
