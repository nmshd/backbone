using Backbone.Modules.Challenges.Domain.Entities;
using Backbone.Modules.Challenges.Domain.Ids;

namespace Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
public interface IChallengesRepository
{
    Task<Challenge> Find(ChallengeId id, CancellationToken cancellationToken);

    Task<Challenge> Add(Challenge challenge, CancellationToken cancellationToken);

    Task DeleteExpiredChallenges(List<ChallengeId> idsOfExpiredChallenges, CancellationToken cancellationToken);

    Task<List<ChallengeId>> FindExpiredChallenges(CancellationToken cancellationToken);
}