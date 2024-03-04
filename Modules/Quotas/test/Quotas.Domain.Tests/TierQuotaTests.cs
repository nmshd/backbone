using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.UnitTestTools.Data;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;

public class TierQuotaTests
{
    [Fact]
    public void Can_create_tier_quota_with_valid_properties()
    {
        // Arrange
        var max = 5;
        var metricKey = MetricKey.NumberOfSentMessages;
        var period = QuotaPeriod.Month;
        var tierQuotaDefinition = new TierQuotaDefinition(metricKey, max, period);
        var applyTo = TestDataGenerator.CreateRandomIdentityAddress();

        // Act
        var tierQuota = new TierQuota(tierQuotaDefinition, applyTo);

        // Assert
        tierQuota.Id.Should().NotBeNull();
        tierQuota.ApplyTo.Should().Be(applyTo);
        tierQuota.MetricKey.Should().Be(metricKey);
        tierQuota.Period.Should().Be(period);
        tierQuota.Max.Should().Be(max);
        tierQuota.Weight.Should().Be(1);
    }
}
