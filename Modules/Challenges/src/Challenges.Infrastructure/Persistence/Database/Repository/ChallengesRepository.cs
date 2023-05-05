using Backbone.Modules.Challenges.Application.Extensions;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Challenges.Domain.Entities;
using Backbone.Modules.Challenges.Domain.Ids;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Challenges.Infrastructure.Persistence.Database.Repository;
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
            .NotExpired()
            .FirstWithId(id, cancellationToken);
    }

    public async Task<Challenge> Add(Challenge challenge, CancellationToken cancellationToken)
    {
        var add = await _challenges.AddAsync(challenge, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return add.Entity;
    }

    public async Task<List<ChallengeId>> FindExpiredChallenges(CancellationToken cancellationToken)
    {
        var idsOfExpiredChallenges = await _dbContext
            .Set<Challenge>()
            .Where(Challenge.CanBeCleanedUp)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        return idsOfExpiredChallenges;
    }

    public async Task DeleteExpiredChallenges(List<ChallengeId> idsOfExpiredChallenges, CancellationToken cancellationToken)
    {
        _challenges.RemoveRange(idsOfExpiredChallenges.Select(id => new Challenge { Id = id }));
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
