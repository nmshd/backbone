﻿using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Metrics;
public abstract class MetricCalculatorFactory
{
    public IMetricCalculator CreateFor(MetricKey metricKey)
    {
        if (metricKey == MetricKey.NumberOfSentMessages)
        {
            return CreateNumberOfSentMessagesMetricCalculator();
        }

        throw new NotSupportedException($"There is currently no {nameof(IMetricCalculator)} for the Metric with the key '{metricKey}'.");
    }

    public abstract IMetricCalculator CreateNumberOfSentMessagesMetricCalculator();
}
