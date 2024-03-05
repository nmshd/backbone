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

        if (metricKey == MetricKey.NumberOfRelationships)
        {
            return CreateNumberOfRelationshipsMetricCalculator();
        }

        if (metricKey == MetricKey.NumberOfRelationshipTemplates)
        {
            return CreateNumberOfRelationshipTemplatesMetricCalculator();
        }

        if (metricKey == MetricKey.NumberOfTokens)
        {
            return CreateNumberOfTokensMetricCalculator();
        }

        if (metricKey == MetricKey.UsedFileStorageSpace)
        {
            return CreateUsedFileStorageSpaceCalculator();
        }

        throw new NotSupportedException($"There is currently no {nameof(IMetricCalculator)} for the Metric with the key '{metricKey}'.");
    }

    protected abstract IMetricCalculator CreateNumberOfFilesMetricCalculator();
    protected abstract IMetricCalculator CreateNumberOfSentMessagesMetricCalculator();
    protected abstract IMetricCalculator CreateNumberOfRelationshipsMetricCalculator();
    protected abstract IMetricCalculator CreateNumberOfRelationshipTemplatesMetricCalculator();
    protected abstract IMetricCalculator CreateNumberOfTokensMetricCalculator();
    protected abstract IMetricCalculator CreateUsedFileStorageSpaceCalculator();
}
