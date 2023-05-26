using Backbone.Modules.Tokens.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Enmeshed.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

[ApplyQuotasForMetrics("NumberOfSentMessages")]
public class CreateTokenCommand : IRequest<CreateTokenResponse>, IMapTo<Token>
{
    public byte[] Content { get; set; }
    public DateTime ExpiresAt { get; set; }
}
