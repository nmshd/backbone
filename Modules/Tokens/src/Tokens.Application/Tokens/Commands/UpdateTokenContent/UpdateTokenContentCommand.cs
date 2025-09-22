using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.UpdateTokenContent;

public class UpdateTokenContentCommand : IRequest<UpdateTokenContentResponse>
{
    public required string TokenId { get; init; }
    public required byte[] NewContent { get; init; }
    public required byte[]? Password { get; init; }
}
