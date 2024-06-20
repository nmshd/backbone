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

    public async Task<uint> Count(string identityAddress, DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var count = await _readOnlyChallenges
            .CountAsync(c => c.ExpiresAt.AddMinutes(-Challenge.EXPIRY_TIME_IN_MINUTES) > from && c.ExpiresAt.AddMinutes(-Challenge.EXPIRY_TIME_IN_MINUTES) < to && c.CreatedBy == identityAddress,
                cancellationToken);

        return (uint)count;
    }
}
