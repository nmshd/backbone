using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public abstract class Quota
{
    // ReSharper disable once UnusedMember.Local
    protected Quota()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
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

    internal ExhaustionDate CalculateExhaustion(uint newUsage)
    {
        if (newUsage >= Max)
            return new ExhaustionDate(Period.CalculateEnd());

        return ExhaustionDate.Unexhausted;
    }
}
