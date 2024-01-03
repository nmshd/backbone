using Backbone.Modules.Quotas.Domain;
using Backbone.Modules.Quotas.Domain.Metrics;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities.ListQuotasForIdentity;

public class FakeMetricCalculatorFactory(int? value = null) : MetricCalculatorFactory
{
    protected override IMetricCalculator CreateNumberOfFilesMetricCalculator()
    {
        return value.HasValue ? new FakeMetricCalculator(value.Value) : throw new ArgumentNullException(nameof(value));
    }

    protected override IMetricCalculator CreateNumberOfSentMessagesMetricCalculator()
    {
        return value.HasValue ? new FakeMetricCalculator(value.Value) : throw new ArgumentNullException(nameof(value));
    }

    protected override IMetricCalculator CreateNumberOfRelationshipsMetricCalculator()
    {
        return value.HasValue ? new FakeMetricCalculator(value.Value) : throw new ArgumentNullException(nameof(value));
    }

    protected override IMetricCalculator CreateNumberOfRelationshipTemplatesMetricCalculator()
    {
        return value.HasValue ? new FakeMetricCalculator(value.Value) : throw new ArgumentNullException(nameof(value));
    }

    protected override IMetricCalculator CreateNumberOfTokensMetricCalculator()
    {
        return value.HasValue ? new FakeMetricCalculator(value.Value) : throw new ArgumentNullException(nameof(value));
    }

    protected override IMetricCalculator CreateUsedFileStorageSpaceCalculator()
    {
        return value.HasValue ? new FakeMetricCalculator(value.Value) : throw new ArgumentNullException(nameof(value));
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
