using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.BuildingBlocks.Application.Attributes;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

[ApplyQuotasForMetrics("NumberOfTokens")]
public class CreateTokenCommand : IRequest<CreateTokenResponse>, IMapTo<Token>
{
    public byte[] Content { get; set; }
    public DateTime ExpiresAt { get; set; }
}
