using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

[ApplyQuotasForMetrics("NumberOfTokens")]
public class CreateTokenCommand : IRequest<CreateTokenResponse>
{
    public required byte[] Content { get; set; }
    public required DateTime ExpiresAt { get; set; }
}
