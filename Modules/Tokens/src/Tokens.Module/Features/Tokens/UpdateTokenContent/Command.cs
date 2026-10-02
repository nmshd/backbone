using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.UpdateTokenContent;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("UpdateTokenContentCommand")]
public class Command : IRequest<Response>
{
    public required string TokenId { get; init; }
    public required byte[] NewContent { get; init; }
    public required byte[]? Password { get; init; }
}
