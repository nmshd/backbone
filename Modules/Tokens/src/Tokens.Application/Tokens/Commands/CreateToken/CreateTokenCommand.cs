using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

[ApplyQuotasForMetrics("NumberOfTokens")]
public class CreateTokenCommand : IRequest<CreateTokenResponse>
{
    public byte[]? Content { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public string? ForIdentity { get; init; }
    public byte[]? Password { get; init; }
}
