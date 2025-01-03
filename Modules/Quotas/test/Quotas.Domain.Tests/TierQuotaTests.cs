using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Domain.Tests;

public class TierQuotaTests : AbstractTestsBase
{
    [Fact]
    public void Can_create_tier_quota_with_valid_properties()
    {
        // Arrange
        const int max = 5;
        var metricKey = MetricKey.NUMBER_OF_SENT_MESSAGES;
        const QuotaPeriod period = QuotaPeriod.Month;
        var tierQuotaDefinition = new TierQuotaDefinition(metricKey, max, period);
        var applyTo = CreateRandomIdentityAddress();

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
