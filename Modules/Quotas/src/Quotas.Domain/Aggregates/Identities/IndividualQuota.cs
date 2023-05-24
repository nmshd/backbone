using System.ComponentModel.DataAnnotations.Schema;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

[NotMapped]
public class IndividualQuota : Quota
{
    public IndividualQuota(Metric metric, int max, QuotaPeriod period)
    {
        Metric = metric;
        Max = max;
        Period = period;
    }

    public int Weight => 2;
    public Metric Metric { get; }
    public int Max { get; }
    public QuotaPeriod Period { get; }
}
