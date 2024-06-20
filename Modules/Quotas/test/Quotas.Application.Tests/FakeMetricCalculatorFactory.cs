using Backbone.Modules.Quotas.Domain;
using Backbone.Modules.Quotas.Domain.Metrics;

namespace Backbone.Modules.Quotas.Application.Tests;

public class FakeMetricCalculatorFactory(
    int? numberOfFiles = null,
    int? numberOfSentMessages = null,
    int? numberOfRelationships = null,
    int? numberOfRelationshipTemplates = null,
    int? numberOfTokens = null,
    int? amountOfUsedFileStorageSpace = null,
    int? numberOfStartedDeletionProcesses = null,
    int? numberOfDatawalletModifications = null,
    int? numberOfCreatedDevices = null,
    int? numberOfCreatedChallenges = null) : MetricCalculatorFactory
{
    protected override IMetricCalculator CreateNumberOfFilesMetricCalculator()
    {
        return numberOfFiles.HasValue ? new FakeMetricCalculator(numberOfFiles.Value) : throw new ArgumentNullException(nameof(numberOfFiles));
    }

    protected override IMetricCalculator CreateNumberOfSentMessagesMetricCalculator()
    {
        return numberOfSentMessages.HasValue ? new FakeMetricCalculator(numberOfSentMessages.Value) : throw new ArgumentNullException(nameof(numberOfSentMessages));
    }

    protected override IMetricCalculator CreateNumberOfRelationshipsMetricCalculator()
    {
        return numberOfRelationships.HasValue ? new FakeMetricCalculator(numberOfRelationships.Value) : throw new ArgumentNullException(nameof(numberOfRelationships));
    }

    protected override IMetricCalculator CreateNumberOfRelationshipTemplatesMetricCalculator()
    {
        return numberOfRelationshipTemplates.HasValue ? new FakeMetricCalculator(numberOfRelationshipTemplates.Value) : throw new ArgumentNullException(nameof(numberOfRelationshipTemplates));
    }

    protected override IMetricCalculator CreateNumberOfTokensMetricCalculator()
    {
        return numberOfTokens.HasValue ? new FakeMetricCalculator(numberOfTokens.Value) : throw new ArgumentNullException(nameof(numberOfTokens));
    }

    protected override IMetricCalculator CreateUsedFileStorageSpaceCalculator()
    {
        return amountOfUsedFileStorageSpace.HasValue ? new FakeMetricCalculator(amountOfUsedFileStorageSpace.Value) : throw new ArgumentNullException(nameof(amountOfUsedFileStorageSpace));
    }

    protected override IMetricCalculator CreateNumberOfStartedDeletionProcessesCalculator()
    {
        return numberOfStartedDeletionProcesses.HasValue ? new FakeMetricCalculator(numberOfStartedDeletionProcesses.Value) : throw new ArgumentNullException(nameof(numberOfStartedDeletionProcesses));
    }

    protected override IMetricCalculator CreateNumberOfCreatedDatawalletModificationsCalculator()
    {
        return numberOfDatawalletModifications.HasValue ? new FakeMetricCalculator(numberOfDatawalletModifications.Value) : throw new ArgumentNullException(nameof(numberOfDatawalletModifications));
    }

    protected override IMetricCalculator CreateNumberOfCreatedDevicesCalculator()
    {
        return numberOfCreatedDevices.HasValue ? new FakeMetricCalculator(numberOfCreatedDevices.Value) : throw new ArgumentNullException(nameof(numberOfCreatedDevices));
    }

    protected override IMetricCalculator CreateNumberOfCreatedChallengesCalculator()
    {
        return numberOfCreatedChallenges.HasValue ? new FakeMetricCalculator(numberOfCreatedChallenges.Value) : throw new ArgumentNullException(nameof(numberOfCreatedChallenges));
    }
}

public class FakeMetricCalculator(int value) : IMetricCalculator
{
    public Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        return Task.FromResult((uint)value);
    }
}
