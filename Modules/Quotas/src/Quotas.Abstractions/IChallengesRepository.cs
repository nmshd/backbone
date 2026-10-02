using System.Linq.Expressions;
using Backbone.Modules.Quotas.Domain.Aggregates.Challenges;

namespace Backbone.Modules.Quotas.Abstractions;

public interface IChallengesRepository
{
    Task<uint> Count(Expression<Func<Challenge, bool>> filter, CancellationToken cancellationToken);
}
