using System.Linq.Expressions;
using Backbone.Modules.Challenges.Application.Extensions;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Challenges.Domain.Entities;
using Backbone.Modules.Challenges.Domain.Ids;
using Backbone.Modules.Challenges.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Challenges.Infrastructure.Persistence.Repository;

public class ChallengesRepository : IChallengesRepository
{
    private readonly DbSet<Challenge> _challenges;
    private readonly ChallengesDbContext _dbContext;

    public ChallengesRepository(ChallengesDbContext dbContext)
    {
        _challenges = dbContext.Challenges;
        _dbContext = dbContext;
    }

    public async Task<Challenge> Find(ChallengeId id, CancellationToken cancellationToken)
    {
        return await _challenges
            .FirstWithId(id, cancellationToken);
    }

    public async Task Add(Challenge challenge, CancellationToken cancellationToken)
    {
        await _challenges.AddAsync(challenge, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> DeleteExpiredChallenges(CancellationToken cancellationToken)
    {
        return await _challenges.Where(Challenge.CanBeCleanedUp).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task Delete(Expression<Func<Challenge, bool>> filter, CancellationToken cancellationToken)
    {
        await _challenges.Where(filter).ExecuteDeleteAsync(cancellationToken);
    }
}
