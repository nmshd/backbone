using Backbone.BuildingBlocks.Domain;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public abstract class Quota : Entity
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected Quota()
    {
        Id = null!;
        ApplyTo = null!;
    }

    protected Quota(string applyTo)
    {
        Id = QuotaId.New();
        ApplyTo = applyTo;
    }

    public QuotaId Id { get; }
    public string ApplyTo { get; }
    public abstract int Weight { get; }
    public abstract MetricKey MetricKey { get; }
    public abstract int Max { get; }
    public abstract QuotaPeriod Period { get; }

    internal ExhaustionDate CalculateExhaustion(uint newUsage, DateTime utcNow)
    {
        if (newUsage >= Max)
            return new ExhaustionDate(Period.CalculateEnd(utcNow));

        return ExhaustionDate.UNEXHAUSTED;
    }
}
