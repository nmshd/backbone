using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Metrics;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.Tooling;
using Enmeshed.UnitTestTools.Data;
using Enmeshed.UnitTestTools.Extensions;
using FluentAssertions;
using Xunit;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

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
        var tierQuotaDefinition2 = new TierQuotaDefinition(MetricKey.UsedFileStorageSpace, 2, QuotaPeriod.Hour);

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
    public void Can_delete_individual_quota_by_id()
    {
        // Arrange
        var identity = new Identity("some-address", new TierId("some-tier-id"));
        var createdQuota = identity.CreateIndividualQuota(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Day);

        // Act
        identity.DeleteIndividualQuota(createdQuota.Id);

        // Assert
        identity.IndividualQuotas.Should().HaveCount(0);
    }

    [Fact]
    public void Can_delete_individual_quota_by_id_with_multiple_quotas()
    {
        // Arrange
        var identity = new Identity("some-address", new TierId("some-tier-id"));
        var firstQuota = identity.CreateIndividualQuota(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Day);
        var secondQuota = identity.CreateIndividualQuota(MetricKey.NumberOfFiles, 1, QuotaPeriod.Day);

        // Act
        identity.DeleteIndividualQuota(firstQuota.Id);

        // Assert
        identity.IndividualQuotas.Should().HaveCount(1);
        identity.IndividualQuotas.Should().Contain(secondQuota);
    }

    [Fact]
    public void Trying_to_delete_inexistent_individual_quota_throws_DomainException()
    {
        // Arrange
        var identity = new Identity("some-address", new TierId("some-tier-id"));

        // Act
        var result = identity.DeleteIndividualQuota(QuotaId.Generate());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.recordNotFound");
        result.Error.Message.Should().StartWith("IndividualQuota");
    }


    [Fact]
    public void Can_delete_tier_quota_by_definition_id()
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
    public void Can_delete_tier_quota_by_definition_id_with_multiple_quotas()
    {
        // Arrange
        var identity = new Identity("some-address", new TierId("some-tier-id"));
        var tierQuotaDefinition1 = new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Day);
        var tierQuotaDefinition2 = new TierQuotaDefinition(MetricKey.NumberOfSentMessages, 1, QuotaPeriod.Day);
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition1);
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition2);

        // Act
        identity.DeleteTierQuotaFromDefinitionId(tierQuotaDefinition1.Id);

        // Assert
        identity.TierQuotas.Should().HaveCount(1);
        identity.TierQuotas.ElementAt(0).DefinitionId.Should().Be(tierQuotaDefinition2.Id);
    }

    [Fact]
    public void Trying_to_delete_inexistent_quota_throws_DomainException()
    {
        // Arrange
        var identity = new Identity("some-address", new TierId("some-tier-id"));

        // Act
        var acting = () => identity.DeleteTierQuotaFromDefinitionId(TierQuotaDefinitionId.Create("TQDsomeInexistentIdx").Value);

        // Assert
        acting.Should().Throw<DomainException>();
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

    [Fact]
    public void Creating_a_quota_with_duplicate_quota_metric_period_throws_domain_exception()
    {
        // Arrange
        var metricKey = MetricKey.NumberOfSentMessages;
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var identity = new Identity(identityAddress, new TierId("tier-id"));
        identity.CreateIndividualQuota(metricKey, 5, QuotaPeriod.Hour);

        // Act
        var acting = () => identity.CreateIndividualQuota(metricKey, 5, QuotaPeriod.Hour);

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.quotas.duplicateQuota");
    }

    [Fact]
    public async Task Updates_identity_with_new_tier()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var identity = new Identity(identityAddress, new TierId("tier-id"));
        var newTier = new Tier(new TierId("new-tier-id"), "New Tier");

        // Act
        await identity.ChangeTier(newTier, new MetricCalculatorFactoryStub(0), CancellationToken.None);

        // Assert

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

        protected override IMetricCalculator CreateNumberOfFilesMetricCalculator()
        {
            return _calculator;
        }

        protected override IMetricCalculator CreateNumberOfRelationshipsMetricCalculator()
        {
            return _calculator;
        }
        protected override IMetricCalculator CreateNumberOfRelationshipTemplatesMetricCalculator()
        {
            return _calculator;
        }

        protected override IMetricCalculator CreateNumberOfSentMessagesMetricCalculator()
        {
            return _calculator;
        }

        protected override IMetricCalculator CreateNumberOfTokensMetricCalculator()
        {
            return _calculator;
        }

        protected override IMetricCalculator CreateUsedFileStorageSpaceCalculator()
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
