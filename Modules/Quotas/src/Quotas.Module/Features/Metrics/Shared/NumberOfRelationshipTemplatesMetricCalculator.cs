using Backbone.Modules.Quotas.Abstractions;
using Backbone.Modules.Quotas.Domain;

namespace Backbone.Modules.Quotas.Module.Features.Metrics.Shared;

public class NumberOfRelationshipTemplatesMetricCalculator : IMetricCalculator
{
    private readonly IRelationshipTemplatesRepository _relationshipTemplatesRepository;

    public NumberOfRelationshipTemplatesMetricCalculator(IRelationshipTemplatesRepository relationshipTemplatesRepository)
    {
        _relationshipTemplatesRepository = relationshipTemplatesRepository;
    }

    public async Task<uint> CalculateUsage(DateTime from, DateTime to, string createdBy, CancellationToken cancellationToken)
    {
        var numberOfTemplates = await _relationshipTemplatesRepository.Count(createdBy, from, to, cancellationToken);
        return numberOfTemplates;
    }
}
