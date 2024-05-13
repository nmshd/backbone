using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public class TierQuotaDefinition : Entity<TierQuotaDefinitionId>
{
    private TierQuotaDefinition()
    {
        MetricKey = null!;
    }

    public TierQuotaDefinition(MetricKey metricKey, int max, QuotaPeriod period) : base(TierQuotaDefinitionId.Generate())
    {
        MetricKey = metricKey;
        Max = max;
        Period = period;
    }

    public MetricKey MetricKey { get; }
    public int Max { get; }
    public QuotaPeriod Period { get; }
}
