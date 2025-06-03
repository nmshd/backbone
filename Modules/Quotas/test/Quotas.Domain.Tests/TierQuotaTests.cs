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
        tierQuota.Id.ShouldNotBeNull();
        tierQuota.ApplyTo.ShouldBe(applyTo);
        tierQuota.MetricKey.ShouldBe(metricKey);
        tierQuota.Period.ShouldBe(period);
        tierQuota.Max.ShouldBe(max);
        tierQuota.Weight.ShouldBe(1);
    }
}
