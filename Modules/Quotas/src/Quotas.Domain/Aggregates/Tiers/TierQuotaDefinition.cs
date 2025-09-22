using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public class TierQuotaDefinition : Entity
{
    protected TierQuotaDefinition()
    {
        Id = null!;
        MetricKey = null!;
    }

    public TierQuotaDefinition(MetricKey metricKey, int max, QuotaPeriod period)
    {
        Id = TierQuotaDefinitionId.New();
        MetricKey = metricKey;
        Max = max;
        Period = period;
    }

    public TierQuotaDefinitionId Id { get; }
    public MetricKey MetricKey { get; }
    public int Max { get; }
    public QuotaPeriod Period { get; }
}
