using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.ResetAccessFailedCountOfToken;

public class ResetAccessFailedCountOfTokenCommand : IRequest
{
    public ResetAccessFailedCountOfTokenCommand(string tokenId)
    {
        TokenId = tokenId;
    }

    public string TokenId { get; set; }
}
