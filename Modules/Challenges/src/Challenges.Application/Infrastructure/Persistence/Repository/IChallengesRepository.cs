using Backbone.Challenges.Domain.Entities;
using Backbone.Challenges.Domain.Ids;

namespace Backbone.Challenges.Application.Infrastructure.Persistence.Repository;
public interface IChallengesRepository
{
    Task<Challenge> Find(ChallengeId id, CancellationToken cancellationToken);

    Task Add(Challenge challenge, CancellationToken cancellationToken);

    Task<int> DeleteExpiredChallenges(CancellationToken cancellationToken);
}
