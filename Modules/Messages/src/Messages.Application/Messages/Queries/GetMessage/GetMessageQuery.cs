using Backbone.Messages.Application.Messages.DTOs;
using Backbone.Messages.Domain.Ids;
using MediatR;

namespace Backbone.Messages.Application.Messages.Queries.GetMessage;

public class GetMessageQuery : IRequest<MessageDTO>
{
    public MessageId Id { get; init; }
    public bool NoBody { get; init; }
}
