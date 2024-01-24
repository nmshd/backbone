using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public class TierQuotaDefinition
{
    private TierQuotaDefinition() { }

    public TierQuotaDefinition(MetricKey metricKey, int max, QuotaPeriod period)
    {
        Id = TierQuotaDefinitionId.Generate();
        MetricKey = metricKey;
        Max = max;
        Period = period;
    }

    public TierQuotaDefinitionId Id { get; } = null!;
    public MetricKey MetricKey { get; } = null!;
    public int Max { get; }
    public QuotaPeriod Period { get; }
}
