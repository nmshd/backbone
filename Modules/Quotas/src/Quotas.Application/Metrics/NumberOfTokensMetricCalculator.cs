using Backbone.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Quotas.Domain;

namespace Backbone.Quotas.Application.Metrics;
public class NumberOfTokensMetricCalculator : IMetricCalculator
{
    private readonly ITokensRepository _tokensRepository;

    public NumberOfTokensMetricCalculator(ITokensRepository tokensRepository)
    {
        _tokensRepository = tokensRepository;
    }

    public async Task<uint> CalculateUsage(DateTime from, DateTime to, string createdBy, CancellationToken cancellationToken)
    {
        var numberOfTokens = await _tokensRepository.Count(createdBy, from, to, cancellationToken);
        return numberOfTokens;
    }
}
