using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests.Tests.Tiers;

public class TierTests
{
    [Fact]
    public void Can_create_tier_with_valid_properties()
    {
        // Act
        var tier = new Tier(new TierId("SomeTierId"), "some tier");

        // Assert
        tier.Id.Should().Be(new TierId("SomeTierId"));
        tier.Name.Should().Be("some tier");
    }

    [Fact]
    public void Can_create_quota_on_tier()
    {
        // Arrange
        var tier = new Tier(new TierId("SomeTierId"), "some tier");

        // Act
        tier.CreateQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);

        // Assert
        tier.Quotas.Should().HaveCount(1);
    }
}
