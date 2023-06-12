using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.Tooling;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public abstract class Quota
{
    protected Quota() { }

    protected Quota(string applyTo)
    {
        Id = QuotaId.New();
        ApplyTo = applyTo;
    }

    public QuotaId Id { get; }
    public string ApplyTo { get; }
    public DateTime? IsExhaustedUntil { get; private set; }
    public abstract int Weight { get; }
    public abstract MetricKey MetricKey { get; }
    public abstract int Max { get; }
    public abstract QuotaPeriod Period { get; }
    public DateTime? PeriodEndTime => Period.CalculateEnd();

    // TODO: What is the definition of a Valid quota?
    public bool IsCurrentlyValid()
    {
        throw new NotImplementedException();
    }

    public void UpdateExhaustion(uint newValue)
    {
        if (newValue >= Max)
        {
            IsExhaustedUntil = PeriodEndTime;
        }
        else
        {
            IsExhaustedUntil = null;
        }
    }
}
