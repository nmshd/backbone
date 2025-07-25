using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class TierQuota : Quota
{
    // ReSharper disable once UnusedMember.Local
    protected TierQuota()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Definition = null!;
        DefinitionId = null!;
    }

    public TierQuota(TierQuotaDefinition definition, string applyTo) : base(applyTo)
    {
        Definition = definition;
        DefinitionId = definition.Id;
    }

    protected virtual TierQuotaDefinition Definition { get; }
    public override int Weight => 1;
    public override MetricKey MetricKey => Definition.MetricKey;
    public override int Max => Definition.Max;
    public override QuotaPeriod Period => Definition.Period;
    public TierQuotaDefinitionId DefinitionId { get; }
}
