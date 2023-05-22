using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public class TierQuotaDefinition
{
    public TierQuotaDefinitionId Id { get; }
    public Metric Metric { get; }
    public int Max { get; }
    public QuotaPeriod Period { get; }

    public TierQuotaDefinition(Metric metric, int max, QuotaPeriod period)
    {
        Id = TierQuotaDefinitionId.Generate();
        Metric = metric;
        Max = max;
        Period = period;
    }
}
