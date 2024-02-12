using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class TierQuota : Quota
{
    private readonly TierQuotaDefinition _definition;

    // ReSharper disable once UnusedMember.Local
    private TierQuota()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        _definition = null!;
    }

    public TierQuota(TierQuotaDefinition definition, string applyTo) : base(applyTo)
    {
        _definition = definition;
    }

    public override int Weight => 1;
    public override MetricKey MetricKey => _definition.MetricKey;
    public override int Max => _definition.Max;
    public override QuotaPeriod Period => _definition.Period;
    public TierQuotaDefinitionId DefinitionId => _definition.Id;
}
