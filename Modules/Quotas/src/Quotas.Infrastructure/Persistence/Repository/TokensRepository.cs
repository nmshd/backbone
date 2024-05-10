using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tokens;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
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
        var tokensCount = await _tokensReadOnly
            .CreatedInInterval(createdAtFrom, createdAtTo)
            .CountAsync(t => t.CreatedBy == createdBy.Value, cancellationToken);
        return (uint)tokensCount;
    }
}
