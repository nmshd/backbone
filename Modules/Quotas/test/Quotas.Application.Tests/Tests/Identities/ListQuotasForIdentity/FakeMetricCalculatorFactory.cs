using Backbone.Modules.Quotas.Domain;
using Backbone.Modules.Quotas.Domain.Metrics;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities.ListQuotasForIdentity;

public class FakeMetricCalculatorFactory : MetricCalculatorFactory
{
    private readonly int? _numberOfFiles;
    private readonly int? _numberOfSentMessages;
    private readonly int? _numberOfRelationships;
    private readonly int? _numberOfRelationshipTemplates;
    private readonly int? _numberOfTokens;
    private readonly int? _amountOfUsedFileStorageSpace;

    public FakeMetricCalculatorFactory(int? numberOfFiles = null, int? numberOfSentMessages = null, int? numberOfRelationships = null,
        int? numberOfRelationshipTemplates = null, int? numberOfTokens = null, int? amountOfUsedFileStorageSpace = null)
    {
        _numberOfFiles = numberOfFiles;
        _numberOfSentMessages = numberOfSentMessages;
        _numberOfRelationships = numberOfRelationships;
        _numberOfRelationshipTemplates = numberOfRelationshipTemplates;
        _numberOfTokens = numberOfTokens;
        _amountOfUsedFileStorageSpace = amountOfUsedFileStorageSpace;
    }

    protected override IMetricCalculator CreateNumberOfFilesMetricCalculator()
    {
        throw new NotImplementedException();
    }

    protected override IMetricCalculator CreateNumberOfSentMessagesMetricCalculator()
    {
        return _numberOfSentMessages.HasValue ? new FakeNumberOfSentMessagesMetricCalculator(_numberOfSentMessages.Value) : throw new ArgumentNullException(nameof(_numberOfSentMessages));
    }

    protected override IMetricCalculator CreateNumberOfRelationshipsMetricCalculator()
    {
        throw new NotImplementedException();
    }

    protected override IMetricCalculator CreateNumberOfRelationshipTemplatesMetricCalculator()
    {
        throw new NotImplementedException();
    }

    protected override IMetricCalculator CreateNumberOfTokensMetricCalculator()
    {
        return _numberOfTokens.HasValue ? new FakeNumberOfTokensMetricCalculator(_numberOfTokens.Value) : throw new ArgumentNullException(nameof(_numberOfTokens));
    }

    protected override IMetricCalculator CreateUsedFileStorageSpaceCalculator()
    {
        throw new NotImplementedException();
    }
}

public class FakeNumberOfSentMessagesMetricCalculator : IMetricCalculator
{
    public FakeNumberOfSentMessagesMetricCalculator(int numberOfSentMessages)
    {
        NumberOfSentMessages = numberOfSentMessages;
    }

    public int NumberOfSentMessages { get; set; }

    public Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        return Task.FromResult((uint)NumberOfSentMessages);
    }
}

public class FakeNumberOfTokensMetricCalculator : IMetricCalculator
{
    public FakeNumberOfTokensMetricCalculator(int numberOfTokens)
    {
        NumberOfTokens = numberOfTokens;
    }

    public int NumberOfTokens { get; set; }

    public Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        return Task.FromResult((uint)NumberOfTokens);
    }
}
