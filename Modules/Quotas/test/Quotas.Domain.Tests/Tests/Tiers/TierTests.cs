using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.UnitTestTools.Extensions;
using FluentAssertions;
using Newtonsoft.Json;
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
    public void Cannot_create_quota_for_queued_for_deletion_tier()
    {
        // Arrange & Act
        var tier = Tier.QUEUED_FOR_DELETION.Clone();
        var result = tier.CreateQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.quotas.cannotCreateOrDeleteQuotaOnQueuedForDeletionTier");
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
    public void Cannot_delete_quota_on_queued_for_deletion_tier()
    {
        // Arrange
        var metrics = new List<Metric>
        {
            new(MetricKey.NumberOfRelationships, "Number of Relationships")
        };
        var tier = Tier.QUEUED_FOR_DELETION.Clone();
        var createdQuotaResults = tier.CreateQuotaForAllMetricsOnQueuedForDeletion(metrics);

        // Act
        var result = tier.DeleteQuota(createdQuotaResults.First().Value.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.quotas.cannotCreateOrDeleteQuotaOnQueuedForDeletionTier");
    }

    [Fact]
    public void Does_only_delete_quota_with_given_id()
    {
        // Arrange
        var tier = new Tier(new TierId("SomeTierId"), "some tier");
        tier.CreateQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);
        tier.CreateQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Week);

        var quotaIdToDelete = tier.Quotas.First().Id;
        var otherQuotaId = tier.Quotas.Second().Id;

        // Act
        tier.DeleteQuota(quotaIdToDelete);

        // Assert
        tier.Quotas.Should().HaveCount(1);
        tier.Quotas.First().Id.Should().Be(otherQuotaId);
    }

    [Fact]
    public void Trying_to_delete_inexistent_quota_fails()
    {
        // Arrange
        var tier = new Tier(new TierId("SomeTierId"), "some tier");

        // Act
        var result = tier.DeleteQuota("SomeInexistentTierQuotaDefinitionId");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Creating_a_quota_with_duplicate_quota_metric_period_throws_domain_exception()
    {
        // Arrange
        var metricKey = MetricKey.NumberOfSentMessages;
        var tier = new Tier(new TierId("SomeTierId"), "some tier");
        tier.CreateQuota(metricKey, 5, QuotaPeriod.Hour);

        // Act
        var result = tier.CreateQuota(metricKey, 5, QuotaPeriod.Hour);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("error.platform.quotas.duplicateQuota");
    }

    [Fact]
    public void Create_quota_for_all_metrics_can_only_be_called_on_queued_for_deletion_tier()
    {
        // Arrange
        var metrics = new List<Metric>
        {
            new(MetricKey.NumberOfRelationships, "Number of Relationships")
        };
        var tier = new Tier(new TierId("SomeTierId"), "some tier");

        // Act
        Action act = () => tier.CreateQuotaForAllMetricsOnQueuedForDeletion(metrics);

        // Assert
        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Method can only be called for the 'Queued for Deletion' tier");
    }

    [Fact]
    public void Create_quota_for_all_metrics_only_creates_missing_quotas()
    {
        // Arrange
        var metrics = new List<Metric>
        {
            new(MetricKey.NumberOfSentMessages, "Number of Sent Messages")
        };

        var tier = Tier.QUEUED_FOR_DELETION.Clone();
        var results = tier.CreateQuotaForAllMetricsOnQueuedForDeletion(metrics).ToList();
        results.First().IsSuccess.Should().BeTrue();
        metrics.Add(new Metric(MetricKey.NumberOfRelationships, "Number of Relationships"));

        // Act
        var createdQuotaResults = tier.CreateQuotaForAllMetricsOnQueuedForDeletion(metrics).ToList();

        // Assert
        createdQuotaResults.Where(result => result.IsFailure).Should().BeEmpty();
        createdQuotaResults.Should().HaveCount(1);
    }
}


file static class TierExtensions
{
    public static Tier Clone(this Tier tier)
    {
        var serialized = JsonConvert.SerializeObject(tier);
        return JsonConvert.DeserializeObject<Tier>(serialized)!;
    }
}
