using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class TierQuota : Quota
{
    internal readonly TierQuotaDefinition Definition;

    private TierQuota() { }

    public TierQuota(TierQuotaDefinition definition, string applyTo) : base(applyTo)
    {
        Definition = definition;
    }

    public override int Weight => 1;
    public override MetricKey MetricKey => Definition.MetricKey;
    public override int Max => Definition.Max;
    public override QuotaPeriod Period => Definition.Period;
    public TierQuotaDefinitionId DefinitionId => Definition.Id;
}
