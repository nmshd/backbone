using Backbone.Modules.Messages.Module.Features.Messages.Shared;
using MediatR;

namespace Backbone.Modules.Messages.Module.Features.Messages.GetMessage;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetMessageQuery")]
public class Query : IRequest<MessageDTO>
{
    public required string Id { get; init; }
    public required bool NoBody { get; init; }
}
