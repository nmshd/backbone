using Backbone.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Devices.Domain.Entities;
using Backbone.Devices.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Devices.Infrastructure.Persistence.Repository;
public class ChallengesRepository : IChallengesRepository
{
    private readonly DbSet<Challenge> _challenges;
    private readonly IQueryable<Challenge> _readonlyChallenges;
    private readonly DevicesDbContext _dbContext;

    public ChallengesRepository(DevicesDbContext dbContext)
    {
        _challenges = dbContext.Challenges;
        _readonlyChallenges = dbContext.Challenges.AsNoTracking();
        _dbContext = dbContext;
    }

    public async Task<Challenge> FindById(string id, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _challenges : _readonlyChallenges)
            .Where(c => c.Id == id).FirstOrDefaultAsync(cancellationToken);
    }
}
