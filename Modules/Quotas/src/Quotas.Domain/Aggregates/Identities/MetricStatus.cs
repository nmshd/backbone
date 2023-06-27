﻿using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class MetricStatus
{
    public MetricKey MetricKey { get; }
    public DateTime? IsExhaustedUntil { get; private set; }
    public string IdentityAddress { get; set; }

    public MetricStatus(MetricKey metricKey, DateTime? isExhaustedUntil)
    {
        MetricKey = metricKey;
        IsExhaustedUntil = isExhaustedUntil;
    }

    public MetricStatus()
    { }

    public void Update(DateTime? isExhaustedUntil)
    {
        IsExhaustedUntil = isExhaustedUntil;
    }
}
