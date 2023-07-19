using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Metrics;
using Enmeshed.Tooling;
using Enmeshed.UnitTestTools.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests.Tests.Identities;

public class IdentityTests
{
    [Fact]
    public void Can_create_identity_with_valid_properties()
    {
        // Act
        var identity = new Identity("some-address", new TierId("some-tier-id"));

        // Assert
        identity.Address.Should().Be("some-address");
        identity.TierId.Should().Be(new TierId("some-tier-id"));
    }

    [Fact]
    public void Can_assign_tier_quota_from_definition_to_identity()
    {
        // Arrange
        var identity = new Identity("some-address", new TierId("some-tier-id"));
        var tierQuotaDefinition1 = new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Day);
        var tierQuotaDefinition2 = new TierQuotaDefinition(MetricKey.FileStorageCapacity, 2, QuotaPeriod.Hour);

        // Act
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition1);
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition2);

        // Assert
        identity.TierQuotas.Should().HaveCount(2);

        identity.TierQuotas.First().MetricKey.Should().Be(tierQuotaDefinition1.MetricKey);
        identity.TierQuotas.First().Max.Should().Be(tierQuotaDefinition1.Max);
        identity.TierQuotas.First().Period.Should().Be(tierQuotaDefinition1.Period);
        identity.TierQuotas.Second().MetricKey.Should().Be(tierQuotaDefinition2.MetricKey);
        identity.TierQuotas.Second().Max.Should().Be(tierQuotaDefinition2.Max);
        identity.TierQuotas.Second().Period.Should().Be(tierQuotaDefinition2.Period);
    }

    [Fact]
    public void Can_delete_tier_quota_by_definition_id_from_identity()
    {
        // Arrange
        var identity = new Identity("some-address", new TierId("some-tier-id"));
        var tierQuotaDefinition = new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Day);
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);

        // Act
        identity.DeleteTierQuotaFromDefinitionId(tierQuotaDefinition.Id);

        // Assert
        identity.TierQuotas.Should().HaveCount(0);
    }

    [Fact]
    public async Task Updating_a_non_existing_MetricStatus_creates_a_MetricStatus()
    {
        // Arrange
        var identity = new Identity("some-address", new TierId("some-tier-id"));

        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Hour));

        // Act
        var metricCalculatorFactoryStub = new MetricCalculatorFactoryStub(new MetricCalculatorStub(0));

        await identity.UpdateMetricStatuses(new[] { MetricKey.NumberOfSentMessages }, metricCalculatorFactoryStub);

        // Assert
        identity.MetricStatuses.Should().HaveCount(1);
        identity.MetricStatuses.First().MetricKey.Should().Be(MetricKey.NumberOfSentMessages);
    }

    [Fact]
    public async Task Updating_a_non_existing_MetricStatus_with_no_corresponding_quotas_sets_exhaustion_date_to_null()
    {
        // Arrange
        var identity = new Identity("some-address", new TierId("some-tier-id"));

        // Act
        await identity.UpdateMetricStatuses(new[] { MetricKey.NumberOfSentMessages }, new MetricCalculatorFactoryStub());

        // Assert
        identity.MetricStatuses.Should().HaveCount(1);
        identity.MetricStatuses.First().MetricKey.Should().Be(MetricKey.NumberOfSentMessages);
        identity.MetricStatuses.First().IsExhaustedUntil.Should().Be(ExhaustionDate.Unexhausted);
    }

    [Fact]
    public async Task Updating_an_existing_MetricStatus_with_no_corresponding_quotas_sets_exhaustion_date_to_null()
    {
        // Arrange
        var identity = new Identity("some-address", new TierId("some-tier-id"));
        await identity.UpdateMetricStatuses(new[] { MetricKey.NumberOfSentMessages }, new MetricCalculatorFactoryStub());

        // Act
        await identity.UpdateMetricStatuses(new[] { MetricKey.NumberOfSentMessages }, new MetricCalculatorFactoryStub());

        // Assert
        identity.MetricStatuses.Should().HaveCount(1);
        identity.MetricStatuses.First().MetricKey.Should().Be(MetricKey.NumberOfSentMessages);
        identity.MetricStatuses.First().IsExhaustedUntil.Should().Be(ExhaustionDate.Unexhausted);
    }

    [Fact]
    public async Task Updating_a_MetricStatus_with_1_exhausted_Quota_sets_its_exhaustion_date_to_the_end_of_the_Quotas_period()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2023-01-15T12:00:00"));

        var identity = new Identity("some-address", new TierId("some-tier-id"));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Hour));

        // Act
        await identity.UpdateMetricStatuses(new[] { MetricKey.NumberOfSentMessages }, new MetricCalculatorFactoryStub(1));

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.Should().BeEndOfHour();
    }

    [Fact]
    public async Task Updating_a_MetricStatus_with_1_exhausted_Quota_and_1_non_exhausted_Quota_sets_its_exhaustion_date_to_the_end_of_the_exhausted_Quotas_period()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2023-01-15T12:00:00"));

        var identity = new Identity("some-address", new TierId("some-tier-id"));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Hour));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 2, QuotaPeriod.Day));

        // Act
        await identity.UpdateMetricStatuses(new[] { MetricKey.NumberOfSentMessages }, new MetricCalculatorFactoryStub(1));

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.Should().BeEndOfHour();

        // To make sure that the order of the added Quotas does not matter, we do the same with reversed order
        // Arrange
        identity = new Identity("some-address", new TierId("some-tier-id"));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 2, QuotaPeriod.Day));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Hour));

        // Act
        await identity.UpdateMetricStatuses(new[] { MetricKey.NumberOfSentMessages }, new MetricCalculatorFactoryStub(1));

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.Should().BeEndOfHour();
    }

    [Fact]
    public async Task Updating_a_MetricStatus_with_multiple_exhausted_Quotas_sets_its_exhaustion_date_to_the_end_of_the_greatest_period()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2023-01-15T12:00:00"));

        var identity = new Identity("some-address", new TierId("some-tier-id"));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Hour));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Day));

        // Act
        await identity.UpdateMetricStatuses(new[] { MetricKey.NumberOfSentMessages }, new MetricCalculatorFactoryStub(1));

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.Should().BeEndOfDay();

        // To make sure that the order of the added Quotas does not matter, we do the same with reversed order
        // Arrange
        identity = new Identity("some-address", new TierId("some-tier-id"));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Day));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Hour));

        // Act
        await identity.UpdateMetricStatuses(new[] { MetricKey.NumberOfSentMessages }, new MetricCalculatorFactoryStub(1));

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.Should().BeEndOfDay();
    }

    [Fact]
    public async Task Updating_an_already_exhausted_MetricStatus_with_an_even_higher_usage_recalculates_exhaustion_date()
    {
        // Even though this case will probably never happen in reality (because once the MetricStatus is exhausted, there will be no more updates), we should still test it

        // Arrange
        SystemTime.Set(DateTime.Parse("2023-01-15T12:00:00"));

        var identity = new Identity("some-address", new TierId("some-tier-id"));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Hour));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 2, QuotaPeriod.Day));

        // first update; only the hourly Quota is exhausted
        await identity.UpdateMetricStatuses(new[] { MetricKey.NumberOfSentMessages }, new MetricCalculatorFactoryStub(1));

        // Act
        // second update; now the daily Quota is exhausted as well
        await identity.UpdateMetricStatuses(new[] { MetricKey.NumberOfSentMessages }, new MetricCalculatorFactoryStub(2));

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.Should().BeEndOfDay();
    }

    [Fact]
    public async Task Updating_a_MetricStatus_that_was_exhausted_in_the_past_with_non_exhausted_Quotas_sets_the_exhaustion_date_to_null()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2023-01-15T12:00:00"));

        var identity = new Identity("some-address", new TierId("some-tier-id"));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Hour));

        // first update; the Quota is exhausted
        await identity.UpdateMetricStatuses(new[] { MetricKey.NumberOfSentMessages }, new MetricCalculatorFactoryStub(1));

        // Act
        // second update; the Quota is not exhausted anymore
        await identity.UpdateMetricStatuses(new[] { MetricKey.NumberOfSentMessages }, new MetricCalculatorFactoryStub(0));

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.Should().Be(ExhaustionDate.Unexhausted);
    }

    private class MetricCalculatorFactoryStub : MetricCalculatorFactory
    {
        private readonly MetricCalculatorStub _calculator;

        public MetricCalculatorFactoryStub()
        {
            _calculator = new MetricCalculatorStub(0);
        }

        public MetricCalculatorFactoryStub(int newUsage)
        {
            _calculator = new MetricCalculatorStub(newUsage);
        }

        public MetricCalculatorFactoryStub(MetricCalculatorStub calculator)
        {
            _calculator = calculator;
        }

        public override IMetricCalculator CreateNumberOfSentMessagesMetricCalculator()
        {
            return _calculator;
        }
    }
}

public class MetricCalculatorStub : IMetricCalculator
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

public static class IdentityExtensions
{
    public static async Task UpdateMetricStatuses(this Identity identity, IEnumerable<MetricKey> metrics, MetricCalculatorFactory factory)
    {
        await identity.UpdateMetricStatuses(metrics, factory, CancellationToken.None);
    }

}
