using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class TierQuota : Quota
{
    private readonly TierQuotaDefinition _definition;

    public TierQuota(TierQuotaDefinition definition, string applyTo) : base(applyTo)
    {
        _definition = definition;
    }

    public override int Weight => 2;
    public override Metric Metric => _definition.Metric;
    public override int Max => _definition.Max;
    public override QuotaPeriod Period => _definition.Period;
}
