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

        if (metricKey == MetricKey.NumberOfStartedDeletionProcesses)
        {
            return CreateNumberOfStartedDeletionProcessesCalculator();
        }

        if (metricKey == MetricKey.NumberOfCreatedChallenges)
        {
            return CreateNumberOfCreatedChallengesCalculator();
        }

        if (metricKey == MetricKey.NumberOfCreatedDevices)
        {
            return CreateNumberOfCreatedDevicesCalculator();
        }

        if (metricKey == MetricKey.NumberOfCreatedDatawalletModifications)
        {
            return CreateNumberOfCreatedDatawalletModificationsCalculator();
        }

        throw new NotSupportedException($"There is currently no {nameof(IMetricCalculator)} for the Metric with the key '{metricKey}'.");
    }

    protected abstract IMetricCalculator CreateNumberOfFilesMetricCalculator();
    protected abstract IMetricCalculator CreateNumberOfSentMessagesMetricCalculator();
    protected abstract IMetricCalculator CreateNumberOfRelationshipsMetricCalculator();
    protected abstract IMetricCalculator CreateNumberOfRelationshipTemplatesMetricCalculator();
    protected abstract IMetricCalculator CreateNumberOfTokensMetricCalculator();
    protected abstract IMetricCalculator CreateUsedFileStorageSpaceCalculator();
    protected abstract IMetricCalculator CreateNumberOfStartedDeletionProcessesCalculator();
    protected abstract IMetricCalculator CreateNumberOfCreatedDatawalletModificationsCalculator();
    protected abstract IMetricCalculator CreateNumberOfCreatedDevicesCalculator();
    protected abstract IMetricCalculator CreateNumberOfCreatedChallengesCalculator();
}
