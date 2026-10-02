using Backbone.Modules.Messages.Module.Features.Messages.Shared;
using MediatR;

namespace Backbone.Modules.Messages.Module.Features.Messages.GetMessage;

public class Query : IRequest<MessageDTO>
{
    public required string Id { get; init; }
    public required bool NoBody { get; init; }
}
