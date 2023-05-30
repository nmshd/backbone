using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public abstract class Quota
{
    public Quota()
    {

    }

    public Quota(string applyTo)
    {
        Id = QuotaId.New();
        ApplyTo = applyTo;
    }

    public QuotaId Id { get; }
    public string ApplyTo { get; }
    public DateTime? IsExhaustedUntil { get; }
    public abstract int Weight { get; }
    public abstract MetricKey MetricKey { get; }
    public abstract int Max { get; }
    public abstract QuotaPeriod Period { get; }
}
