using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class MetricStatus
{
    public MetricKey MetricKey { get; }
    public DateTime? IsExhaustedUntil { get; private set; }
    
    /// <summary>
    /// An IdentityAddress
    /// </summary>
    public string Owner { get; set; }

    public MetricStatus(MetricKey metricKey, DateTime? isExhaustedUntil)
    {
        MetricKey = metricKey;
        IsExhaustedUntil = isExhaustedUntil;
    }

    public void Update(DateTime? isExhaustedUntil)
    {
        IsExhaustedUntil = isExhaustedUntil;
    }
}
