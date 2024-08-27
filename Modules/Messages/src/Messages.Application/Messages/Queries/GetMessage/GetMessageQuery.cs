using Backbone.Modules.Messages.Application.Messages.DTOs;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Queries.GetMessage;

public class GetMessageQuery : IRequest<MessageDTO>
{
    public required string Id { get; init; }
    public required bool NoBody { get; init; }
}
