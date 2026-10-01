using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.CreateToken;

[ApplyQuotasForMetrics("NumberOfTokens")]
public class CreateTokenCommand : IRequest<CreateTokenResponse>
{
    public byte[]? Content { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public string? ForIdentity { get; init; }
    public byte[]? Password { get; init; }
}
