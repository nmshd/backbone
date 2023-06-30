using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class MetricStatus
{
    public MetricKey MetricKey { get; }
    public DateTime? IsExhaustedUntil { get; private set; }
    
    /// <summary>
    /// An IdentityAddress
    /// </summary>
    public string Owner { get; private set; }

    public MetricStatus(MetricKey metricKey, string owner, DateTime? isExhaustedUntil)
    {
        MetricKey = metricKey;
        Owner = owner;
        IsExhaustedUntil = isExhaustedUntil;
    }

    public void Update(DateTime? isExhaustedUntil)
    {
        IsExhaustedUntil = isExhaustedUntil;
    }
}
