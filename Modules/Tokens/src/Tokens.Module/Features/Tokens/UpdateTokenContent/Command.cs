using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.UpdateTokenContent;

public class Command : IRequest<Response>
{
    public required string TokenId { get; init; }
    public required byte[] NewContent { get; init; }
    public required byte[]? Password { get; init; }
}
