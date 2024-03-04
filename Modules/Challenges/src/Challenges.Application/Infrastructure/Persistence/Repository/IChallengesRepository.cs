using System.Linq.Expressions;
using Backbone.Modules.Challenges.Domain.Entities;
using Backbone.Modules.Challenges.Domain.Ids;

namespace Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
public interface IChallengesRepository
{
    Task<Challenge> Find(ChallengeId id, CancellationToken cancellationToken);
    Task Add(Challenge challenge, CancellationToken cancellationToken);
    Task<int> DeleteExpiredChallenges(CancellationToken cancellationToken);
    Task DeleteChallenges(Expression<Func<Challenge, bool>> filter, CancellationToken cancellationToken);
}
