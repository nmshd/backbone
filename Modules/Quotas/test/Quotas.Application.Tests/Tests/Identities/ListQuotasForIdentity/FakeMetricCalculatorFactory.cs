using Backbone.Modules.Quotas.Domain;
using Backbone.Modules.Quotas.Domain.Metrics;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities.ListQuotasForIdentity;

public class FakeMetricCalculatorFactory(
        int? numberOfFiles = null, int? numberOfSentMessages = null, int? numberOfRelationships = null,
        int? numberOfRelationshipTemplates = null, int? numberOfTokens = null, int? amountOfUsedFileStorageSpace = null) : MetricCalculatorFactory
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
}

public class FakeMetricCalculator(int value) : IMetricCalculator
{
    private int Value { get; } = value;

    public Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        return Task.FromResult((uint)Value);
    }
}
