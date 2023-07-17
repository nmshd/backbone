using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Metrics;
public abstract class MetricCalculatorFactory
{
    public IMetricCalculator CreateFor(MetricKey metricKey)
    {
        if (metricKey == MetricKey.NumberOfSentMessages)
        {
            return CreateNumberOfSentMessagesMetricCalculator();
        }

        if (metricKey == MetricKey.NumberOfFiles)
        {
            return CreateNumberOfFilesMetricCalculator();
        }

        throw new NotSupportedException($"There is currently no {nameof(IMetricCalculator)} for the Metric with the key '{metricKey}'.");
    }

    public abstract IMetricCalculator CreateNumberOfFilesMetricCalculator();
    public abstract IMetricCalculator CreateNumberOfSentMessagesMetricCalculator();
}
