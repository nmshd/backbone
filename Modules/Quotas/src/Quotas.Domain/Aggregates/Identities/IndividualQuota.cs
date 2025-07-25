using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class IndividualQuota : Quota
{
    // ReSharper disable once UnusedMember.Local
    protected IndividualQuota()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        MetricKey = null!;
    }

    public IndividualQuota(MetricKey metricKey, int max, QuotaPeriod period, string applyTo) : base(applyTo)
    {
        MetricKey = metricKey;
        Max = max;
        Period = period;
    }

    public override int Weight => 2;
    public override MetricKey MetricKey { get; }
    public override int Max { get; }
    public override QuotaPeriod Period { get; }
}
