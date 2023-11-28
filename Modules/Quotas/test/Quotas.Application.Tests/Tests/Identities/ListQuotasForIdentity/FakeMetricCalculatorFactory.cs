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
        return _numberOfFiles.HasValue ? new FakeNumberOfFilesMetricCalculator(_numberOfFiles.Value) : throw new ArgumentNullException(nameof(_numberOfFiles));
    }

    protected override IMetricCalculator CreateNumberOfSentMessagesMetricCalculator()
    {
        return _numberOfSentMessages.HasValue ? new FakeNumberOfSentMessagesMetricCalculator(_numberOfSentMessages.Value) : throw new ArgumentNullException(nameof(_numberOfSentMessages));
    }

    protected override IMetricCalculator CreateNumberOfRelationshipsMetricCalculator()
    {
        return _numberOfRelationships.HasValue ? new FakeNumberOfRelationshipsMetricCalculator(_numberOfRelationships.Value) : throw new ArgumentNullException(nameof(_numberOfRelationships));
    }

    protected override IMetricCalculator CreateNumberOfRelationshipTemplatesMetricCalculator()
    {
        return _numberOfRelationshipTemplates.HasValue ? new FakeNumberOfRelationshipsMetricCalculator(_numberOfRelationshipTemplates.Value) : throw new ArgumentNullException(nameof(_numberOfRelationshipTemplates));
    }

    protected override IMetricCalculator CreateNumberOfTokensMetricCalculator()
    {
        return _numberOfTokens.HasValue ? new FakeNumberOfTokensMetricCalculator(_numberOfTokens.Value) : throw new ArgumentNullException(nameof(_numberOfTokens));
    }

    protected override IMetricCalculator CreateUsedFileStorageSpaceCalculator()
    {
        return _amountOfUsedFileStorageSpace.HasValue ? new FakeUsedFileStorageSpaceMetricCalculator(_amountOfUsedFileStorageSpace.Value) : throw new ArgumentNullException(nameof(_amountOfUsedFileStorageSpace));
    }
}

public class FakeNumberOfFilesMetricCalculator : IMetricCalculator
{
    public FakeNumberOfFilesMetricCalculator(int numberOfFiles)
    {
        NumberOfFiles = numberOfFiles;
    }

    public int NumberOfFiles { get; set; }

    public Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        return Task.FromResult((uint)NumberOfFiles);
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

public class FakeNumberOfRelationshipsMetricCalculator : IMetricCalculator
{
    public FakeNumberOfRelationshipsMetricCalculator(int numberOfRelationships)
    {
        NumberOfRelationships = numberOfRelationships;
    }

    public int NumberOfRelationships { get; set; }

    public Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        return Task.FromResult((uint)NumberOfRelationships);
    }
}

public class FakeNumberOfRelationshipTemplatesMetricCalculator : IMetricCalculator
{
    public FakeNumberOfRelationshipTemplatesMetricCalculator(int numberOfRelationshipTemplates)
    {
        NumberOfRelationshipTemplates = numberOfRelationshipTemplates;
    }

    public int NumberOfRelationshipTemplates { get; set; }

    public Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        return Task.FromResult((uint)NumberOfRelationshipTemplates);
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

public class FakeUsedFileStorageSpaceMetricCalculator : IMetricCalculator
{
    public FakeUsedFileStorageSpaceMetricCalculator(int amountOfUsedFileStorageSpace)
    {
        AmountOfUsedFileStorageSpace = amountOfUsedFileStorageSpace;
    }

    public int AmountOfUsedFileStorageSpace { get; set; }

    public Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        return Task.FromResult((uint)AmountOfUsedFileStorageSpace);
    }
}
