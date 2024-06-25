using System.Linq.Expressions;
using Backbone.Modules.Quotas.Domain.Aggregates.Challenges;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;

public interface IChallengesRepository
{
    Task<uint> Count(Expression<Func<Challenge, bool>> filter, CancellationToken cancellationToken);
}
