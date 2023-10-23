using Backbone.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Quotas.Domain;

namespace Backbone.Quotas.Application.Metrics;
public class NumberOfRelationshipsMetricCalculator : IMetricCalculator
{
    private readonly IRelationshipsRepository _relationshipsRepository;

    public NumberOfRelationshipsMetricCalculator(IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task<uint> CalculateUsage(DateTime from, DateTime to, string participant, CancellationToken cancellationToken)
    {
        var numberOfRelationships = await _relationshipsRepository.Count(participant, from, to, cancellationToken);
        return numberOfRelationships;
    }
}
