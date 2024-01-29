using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Tokens.DTOs;

public class TokenDTO : IMapTo<Token>
{
    public required TokenId Id { get; set; }

    public required IdentityAddress CreatedBy { get; set; }
    public required DeviceId CreatedByDevice { get; set; }

    public required DateTime CreatedAt { get; set; }
    public required DateTime ExpiresAt { get; set; }

    public required byte[] Content { get; set; }
}
