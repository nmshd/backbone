using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.DTOs;
public class TierDetailsDTO
{
    public TierDetailsDTO(Tier tier, IEnumerable<Metric> metrics)
    {
        Id = tier.Id;
        Name = tier.Name;
        Quotas = tier.Quotas.Select(quota =>
            new TierQuotaDefinitionDTO(
                quota.Id,
                new MetricDTO(metrics.First(metric => metric.Key == quota.MetricKey)),
                quota.Max,
                quota.Period
            )
        );
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<TierQuotaDefinitionDTO> Quotas { get; set; }
}