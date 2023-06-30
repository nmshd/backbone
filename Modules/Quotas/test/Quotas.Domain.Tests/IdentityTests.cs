using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.UnitTestTools.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;

public class IdentityTests
{
    [Fact]
    public void Can_create_identity_with_valid_properties()
    {
        // Arrange
        var address = "someAddress";
        var tierId = new TierId("TIRsomeTierId1111111");

        // Act
        var identity = new Identity(address, tierId);

        // Assert
        identity.Address.Should().Be(address);
        identity.TierId.Should().Be(tierId);
    }

    [Fact]
    public void Can_assign_tier_quota_from_definition_to_identity()
    {
        // Arrange
        var address = "someAddress";
        var tierId = new TierId("TIRsomeTierId1111111");
        var identity = new Identity(address, tierId);
        var max = 5;
        var tierQuotaDefinition = new TierQuotaDefinition(MetricKey.NumberOfSentMessages, max, QuotaPeriod.Month);

        // Act
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);

        // no exception thrown signifies the test worked
    }

    [Fact]
    public async Task Identity_with_unexhausted_quotas_has_valid_quota()
    {
        // Arrange
        var identity = new Identity("some-dummy-address", new TierId("some-tier-id"));
        var metricKey = MetricKey.NumberOfSentMessages;
        var tierQuotaDefinition = new TierQuotaDefinition(metricKey, 1, QuotaPeriod.Hour);
        var metricCalculatorFactoryReturning0 = new MetricCalculatorFactoryStub(0);
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);

        // Act
        await identity.UpdateMetrics(new[] { metricKey }, metricCalculatorFactoryReturning0, CancellationToken.None);

        // Assert
        identity.AllQuotas.Should().HaveCount(1);
        identity.AllQuotas.First().MetricKey.Should().Be(metricKey);
        identity.MetricStatuses.Should().HaveCount(1);
        identity.MetricStatuses.First().MetricKey.Should().Be(metricKey);
        identity.MetricStatuses.First().IsExhaustedUntil.Should().BeNull();
    }

    [Fact]
    public async Task Identity_without_quotas_has_all_MetricStatuses_IsExhaustedUntil_set_to_null()
    {
        // Arrange
        var identity = new Identity("some-dummy-address", new TierId("some-tier-id"));

        var metricCalculatorFactoryReturning1 = new MetricCalculatorFactoryStub(1);

        // Act
        await identity.UpdateMetrics(new[] { MetricKey.NumberOfSentMessages }, metricCalculatorFactoryReturning1, CancellationToken.None);

        // Assert
        identity.AllQuotas.Should().HaveCount(0);
        identity.MetricStatuses.Select(m => m.IsExhaustedUntil).Should().AllBeEquivalentTo<DateTime?>(null);
    }


    [Fact]
    public async Task Identity_with_pre_existing_MetricStatuses_updates_MetricStatuses_on_UpdateMetrics()
    {
        // Arrange
        var identity = new Identity("some-dummy-address", new TierId("some-tier-id"));
        var metricKey = MetricKey.NumberOfSentMessages;
        var metricCalculatorFactoryReturning5 = new MetricCalculatorFactoryStub(5);

        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(metricKey, 1, QuotaPeriod.Hour));
        await identity.UpdateMetrics(new[] { metricKey }, metricCalculatorFactoryReturning5, CancellationToken.None);

        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(metricKey, 3, QuotaPeriod.Day));

        // Act
        await identity.UpdateMetrics(new[] { metricKey }, metricCalculatorFactoryReturning5, CancellationToken.None);

        // Assert
        identity.MetricStatuses.Should().HaveCount(1);
        var metricStatus = identity.MetricStatuses.First();
        metricStatus.MetricKey.Should().Be(metricKey);
        metricStatus.IsExhaustedUntil.Should().BeEndOfDay();
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
