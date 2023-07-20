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

    [Fact]
    public void Can_delete_quota_on_tier()
    {
        // Arrange
        var tier = new Tier(new TierId("SomeTierId"), "some tier");
        tier.CreateQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);

        // Act
        tier.DeleteQuota(tier.Quotas.First().Id);

        // Assert
        tier.Quotas.Should().HaveCount(0);
    }

    [Fact]
    public void Can_delete_quota_on_tier_with_multiple_quotas()
    {
        // Arrange
        var tier = new Tier(new TierId("SomeTierId"), "some tier");
        tier.CreateQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);
        tier.CreateQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);

        var deletedQuotaId = tier.Quotas.ElementAt(0).Id;
        var notDeletedQuotaId = tier.Quotas.ElementAt(1).Id;

        // Act
        tier.DeleteQuota(deletedQuotaId);

        // Assert
        tier.Quotas.Should().HaveCount(1);
        tier.Quotas.ElementAt(0).Id.Should().Be(notDeletedQuotaId);
    }

    [Fact]
    public void Cannot_delete_non_existent_quota_on_tier()
    {
        // Arrange
        var tier = new Tier(new TierId("SomeTierId"), "some tier");

        // Act
        var result = tier.DeleteQuota("SomeInexistentTierQuotaDefinitionId");

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
