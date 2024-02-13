﻿using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class MetricStatus
{
    public MetricKey MetricKey { get; }
    public ExhaustionDate IsExhaustedUntil { get; private set; }

    public string Owner { get; private set; }

    public MetricStatus(MetricKey metricKey, string owner, ExhaustionDate isExhaustedUntil)
    {
        MetricKey = metricKey;
        Owner = owner;
        IsExhaustedUntil = isExhaustedUntil;
    }

    public void Update(ExhaustionDate isExhaustedUntil)
    {
        IsExhaustedUntil = isExhaustedUntil;
    }
}
