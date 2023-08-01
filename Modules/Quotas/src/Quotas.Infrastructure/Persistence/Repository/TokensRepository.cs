using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tokens;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
public class TokensRepository : ITokensRepository
{
    private readonly IQueryable<Token> _tokensReadOnly;

    public TokensRepository(QuotasDbContext dbContext)
    {
        _tokensReadOnly = dbContext.Tokens.AsNoTracking();
    }

    public async Task<uint> Count(IdentityAddress createdBy, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken)
    {
        var tokensCount = await _tokensReadOnly.CountAsync(t => t.CreatedBy == createdBy.StringValue, cancellationToken);
        return (uint)tokensCount;
    }
}
