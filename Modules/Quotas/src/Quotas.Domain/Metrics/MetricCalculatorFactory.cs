using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Metrics;

public abstract class MetricCalculatorFactory
{
    public IMetricCalculator CreateFor(MetricKey metricKey)
    {
        if (metricKey == MetricKey.NUMBER_OF_SENT_MESSAGES)
        {
            return CreateNumberOfSentMessagesMetricCalculator();
        }

        if (metricKey == MetricKey.NUMBER_OF_FILES)
        {
            return CreateNumberOfFilesMetricCalculator();
        }

        if (metricKey == MetricKey.NUMBER_OF_RELATIONSHIPS)
        {
            return CreateNumberOfRelationshipsMetricCalculator();
        }

        if (metricKey == MetricKey.NUMBER_OF_RELATIONSHIP_TEMPLATES)
        {
            return CreateNumberOfRelationshipTemplatesMetricCalculator();
        }

        if (metricKey == MetricKey.NUMBER_OF_TOKENS)
        {
            return CreateNumberOfTokensMetricCalculator();
        }

        if (metricKey == MetricKey.USED_FILE_STORAGE_SPACE)
        {
            return CreateUsedFileStorageSpaceCalculator();
        }

        if (metricKey == MetricKey.NUMBER_OF_STARTED_DELETION_PROCESSES)
        {
            return CreateNumberOfStartedDeletionProcessesCalculator();
        }

        if (metricKey == MetricKey.NUMBER_OF_CREATED_CHALLENGES)
        {
            return CreateNumberOfCreatedChallengesCalculator();
        }

        if (metricKey == MetricKey.NUMBER_OF_CREATED_DEVICES)
        {
            return CreateNumberOfCreatedDevicesCalculator();
        }

        if (metricKey == MetricKey.NUMBER_OF_CREATED_DATAWALLET_MODIFICATIONS)
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
