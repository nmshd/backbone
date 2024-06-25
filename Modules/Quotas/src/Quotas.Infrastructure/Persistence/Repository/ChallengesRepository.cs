using System.Linq.Expressions;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Challenges;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;

public class ChallengesRepository : IChallengesRepository
{
    private readonly IQueryable<Challenge> _readOnlyChallenges;

    public ChallengesRepository(QuotasDbContext dbContext)
    {
        _readOnlyChallenges = dbContext.Challenges.AsNoTracking();
    }

    public async Task<uint> Count(Expression<Func<Challenge, bool>> filter, CancellationToken cancellationToken)
    {
        var count = await _readOnlyChallenges.CountAsync(filter, cancellationToken);
        return (uint)count;
    }
}
