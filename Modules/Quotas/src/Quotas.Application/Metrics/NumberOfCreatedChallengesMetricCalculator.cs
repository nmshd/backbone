using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain;

namespace Backbone.Modules.Quotas.Application.Metrics;

public class NumberOfCreatedChallengesMetricCalculator : IMetricCalculator
{
    private readonly IChallengesRepository _challengesRepository;

    public NumberOfCreatedChallengesMetricCalculator(IChallengesRepository challengesRepository)
    {
        _challengesRepository = challengesRepository;
    }

    public async Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        var numberOfCreatedChallenges = await _challengesRepository.Count(identityAddress, from, to, cancellationToken);
        return numberOfCreatedChallenges;
    }
}
