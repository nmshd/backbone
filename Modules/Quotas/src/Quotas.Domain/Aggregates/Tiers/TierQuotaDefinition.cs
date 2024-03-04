using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public class TierQuotaDefinition
{
    private TierQuotaDefinition()
    {
        Id = null!;
        MetricKey = null!;
    }

    public TierQuotaDefinition(MetricKey metricKey, int max, QuotaPeriod period)
    {
        Id = TierQuotaDefinitionId.Generate();
        MetricKey = metricKey;
        Max = max;
        Period = period;
    }

    public TierQuotaDefinitionId Id { get; }
    public MetricKey MetricKey { get; }
    public int Max { get; }
    public QuotaPeriod Period { get; }
}
