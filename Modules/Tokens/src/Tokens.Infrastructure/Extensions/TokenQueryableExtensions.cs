using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Tooling;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Tokens.Infrastructure.Extensions;

public static class TokenQueryableExtensions
{
    public static async Task<Token?> FirstWithIdOrDefault(this IQueryable<Token> query, TokenId tokenId, CancellationToken cancellationToken)
    {
        var template = await query.FirstOrDefaultAsync(t => t.Id == tokenId, cancellationToken);
        return template;
    }

    public static IQueryable<Token> NotExpiredFor(this IQueryable<Token> query, IdentityAddress address)
    {
        return query.Where(t => t.ExpiresAt > SystemTime.UtcNow);
    }
}
