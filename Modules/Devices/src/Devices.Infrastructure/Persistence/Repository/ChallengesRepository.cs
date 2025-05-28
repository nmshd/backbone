using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;

public class ChallengesRepository : IChallengesRepository
{
    private readonly DbSet<Challenge> _challenges;
    private readonly IQueryable<Challenge> _readonlyChallenges;

    public ChallengesRepository(DevicesDbContext dbContext)
    {
        _challenges = dbContext.Challenges;
        _readonlyChallenges = dbContext.Challenges.AsNoTracking();
    }

    public async Task<Challenge?> GetById(string id, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _challenges : _readonlyChallenges)
            .Where(c => c.Id == id).FirstOrDefaultAsync(cancellationToken);
    }
}
