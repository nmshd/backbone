using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;

public class TierTests
{
    [Fact]
    public void Can_create_tier_with_valid_properties()
    {
        // Arrange
        var tierId = new TierId("TIRsomeTierId1111111");
        var tierName = "my-test-tier";

        // Act
        var tier = new Tier(tierId, tierName);

        // Assert
        tier.Id.Should().Be(tierId);
        tier.Name.Should().Be(tierName);
    }

    [Fact]
    public void Can_create_quota_on_tier()
    {
        // Arrange
        var tierId = new TierId("TIRsomeTierId1111111");
        var tierName = "my-test-tier";
        var tier = new Tier(tierId, tierName);
        var max = 5;

        // Act
        tier.CreateQuota(MetricKey.NumberOfSentMessages, max, QuotaPeriod.Month);

        // Assert
        tier.Quotas.Should().HaveCount(1);
    }
}
