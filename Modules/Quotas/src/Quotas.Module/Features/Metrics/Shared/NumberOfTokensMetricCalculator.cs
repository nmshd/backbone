using Backbone.Modules.Quotas.Abstractions;
using Backbone.Modules.Quotas.Domain;

namespace Backbone.Modules.Quotas.Module.Features.Metrics.Shared;

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
