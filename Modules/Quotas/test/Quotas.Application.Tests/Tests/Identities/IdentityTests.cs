using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities;
public class IdentityTests
{
    [Fact]
    public async Task Identity_with_unexhausted_quotas_has_valid_quota()
    {
        // Arrange
        var identity = new Identity("some-dummy-address", new TierId("some-tier-id"));
        var metricKey = Domain.Aggregates.Metrics.MetricKey.NumberOfSentMessages;
        var tierQuotaDefinition = new TierQuotaDefinition(metricKey, 1, QuotaPeriod.Hour);
        var metricCalculatorFactoryReturning0 = new MetricCalculatorFactoryStub(0);
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);

        // Act
        await identity.UpdateMetrics(new List<MetricKey>() { metricKey }, metricCalculatorFactoryReturning0, CancellationToken.None);

        // Assert
        identity.AllQuotas.Should().HaveCount(1);
        identity.AllQuotas.First().MetricKey.Should().Be(metricKey);
        identity.AllQuotas.First().IsExhaustedUntil.Should().BeNull();
    }

    [Fact]
    public async Task Identity_with_exhausted_quota_has_non_null_IsExhaustedUntil()
    {
        // Arrange
        var identity = new Identity("some-dummy-address", new TierId("some-tier-id"));
        var metricKey = MetricKey.NumberOfSentMessages;
        var tierQuotaDefinition = new TierQuotaDefinition(metricKey, 1, QuotaPeriod.Hour);
        var metricCalculatorFactoryReturning5 = new MetricCalculatorFactoryStub(5);
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);

        // Act
        await identity.UpdateMetrics(new List<MetricKey>(){ metricKey}, metricCalculatorFactoryReturning5, CancellationToken.None);

        // Assert
        identity.AllQuotas.First().MetricKey.Should().Be(metricKey);
        identity.AllQuotas.First().IsExhaustedUntil.Should().NotBeNull();
        identity.AllQuotas.First().IsExhaustedUntil.Value.Hour.Should().Be(SystemTime.UtcNow.Hour);
        identity.AllQuotas.First().IsExhaustedUntil.Value.Minute.Should().Be(59);
        identity.AllQuotas.First().IsExhaustedUntil.Value.Second.Should().Be(59);
    }

    private class MetricCalculatorFactoryStub : MetricCalculatorFactory
    {
        private readonly int _newUsage;

        public MetricCalculatorFactoryStub(int newUsage)
        {
            _newUsage = newUsage;
        }

        public override IMetricCalculator CreateNumberOfSentMessagesMetricCalculator()
        {
            return new MetricCalculatorStub(_newUsage);
        }

        private class MetricCalculatorStub : IMetricCalculator
        {
            private readonly int _newUsage;

            public MetricCalculatorStub(int newUsage)
            {
                _newUsage = newUsage;
            }

            public Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
            {
                return Task.FromResult((uint)_newUsage);
            }
        }
    }

}

