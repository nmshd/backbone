﻿using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

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

    public void UpdateExhaustion(uint newValue) 
    {
        if (newValue >= Max)
        {
            IsExhaustedUntil = Period.CalculateEnd();
        }
        else
        {
            IsExhaustedUntil = null;
        }
    }
}
