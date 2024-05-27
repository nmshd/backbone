using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.Modules.Quotas.Domain.DomainEvents.Outgoing;
using Backbone.UnitTestTools.Extensions;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests.Tests.Tiers;

public class TierTests : AbstractTestsBase
{
    [Fact]
    public void Can_create_tier_with_valid_properties()
    {
        // Act
        var tier = new Tier(TierId.Parse("TIRsomeTierId1111111"), "some tier");

        // Assert
        tier.Id.Should().Be(TierId.Parse("TIRsomeTierId1111111"));
        tier.Name.Should().Be("some tier");
    }

    [Fact]
    public void Can_create_quota_on_tier()
    {
        // Arrange
        var tier = new Tier(TierId.Parse("tier-id"), "some tier");

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
        var tier = new Tier(TierId.Parse("tier-id"), "some tier");
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
        var addedQuotas = tier.AddQuotaForAllMetricsOnQueuedForDeletion(metrics);

        // Act
        var result = tier.DeleteQuota(addedQuotas.First().Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("error.platform.quotas.cannotCreateOrDeleteQuotaOnQueuedForDeletionTier");
    }

    [Fact]
    public void Does_only_delete_quota_with_given_id()
    {
        // Arrange
        var tier = new Tier(TierId.Parse("tier-id"), "some tier");
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
        var tier = new Tier(TierId.Parse("tier-id"), "some tier");

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
        var tier = new Tier(TierId.Parse("tier-id"), "some tier");
        tier.CreateQuota(metricKey, 5, QuotaPeriod.Hour);

        // Act
        var result = tier.CreateQuota(metricKey, 5, QuotaPeriod.Hour);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("error.platform.quotas.duplicateQuota");
    }

    [Fact]
    public void AddQuotaForAllMetricsOnQueuedForDeletion_can_only_be_called_on_queued_for_deletion_tier()
    {
        // Arrange
        var metrics = new List<Metric>
        {
            new(MetricKey.NumberOfRelationships, "Number of Relationships")
        };
        var tier = new Tier(TierId.Parse("tier-id"), "some tier");

        // Act
        Action act = () => tier.AddQuotaForAllMetricsOnQueuedForDeletion(metrics);

        // Assert
        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Method can only be called for the 'Queued for Deletion' tier");
    }

    [Fact]
    public void AddQuotaForAllMetricsOnQueuedForDeletion_adds_quotas()
    {
        // Arrange
        var tier = Tier.QUEUED_FOR_DELETION.Clone();

        var metrics = new List<Metric>
        {
            new(MetricKey.NumberOfSentMessages, "Number of Sent Messages")
        };

        // Act
        var addedQuotas = tier.AddQuotaForAllMetricsOnQueuedForDeletion(metrics).ToList();

        // Assert
        tier.Quotas.Should().HaveCount(1);
        addedQuotas.Should().HaveCount(1);
        addedQuotas.First().MetricKey.Should().Be(MetricKey.NumberOfSentMessages);
    }

    [Fact]
    public void AddQuotaForAllMetricsOnQueuedForDeletion_only_creates_missing_quotas()
    {
        // Arrange
        var metrics = new List<Metric>
        {
            new(MetricKey.NumberOfSentMessages, "Number of Sent Messages")
        };

        var tier = Tier.QUEUED_FOR_DELETION.Clone();
        tier.AddQuotaForAllMetricsOnQueuedForDeletion(metrics);
        metrics.Add(new Metric(MetricKey.NumberOfRelationships, "Number of Relationships"));

        // Act
        var addedQuotas = tier.AddQuotaForAllMetricsOnQueuedForDeletion(metrics).ToList();

        // Assert
        addedQuotas.Should().HaveCount(1);
        tier.Quotas.Should().HaveCount(2);
    }

    [Fact]
    public void Deleting_a_quota_triggers_TierQuotaDefinitionDeletedDomainEvent()
    {
        // Arrange
        var tierId = TierId.Parse("tier-id");
        var tier = new Tier(tierId, "some-tier-name");

        tier.CreateQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);
        var tierQuotaDefinitionId = tier.Quotas.First().Id;
        tier.ClearDomainEvents();

        // Act
        tier.DeleteQuota(tierQuotaDefinitionId);

        // Assert
        var domainEvent = tier.Should().HaveASingleDomainEvent<TierQuotaDefinitionDeletedDomainEvent>();
        domainEvent.TierId.Should().Be(tierId);
        domainEvent.TierQuotaDefinitionId.Should().Be(tierQuotaDefinitionId);
    }

    [Fact]
    public void Creating_a_quota_triggers_TierQuotaDefinitionCreatedDomainEvent()
    {
        // Arrange
        var tierId = TierId.Parse("TIRsomeTierId1111111");
        var metricKey = MetricKey.NumberOfSentMessages;
        var tier = new Tier(tierId, "some-tier-name");

        // Act
        tier.CreateQuota(metricKey, 5, QuotaPeriod.Month);

        // Assert
        var domainEvent = tier.Should().HaveASingleDomainEvent<TierQuotaDefinitionCreatedDomainEvent>();
        domainEvent.TierId.Should().Be(tierId);
    }
}

file static class TierExtensions
{
    public static Tier Clone(this Tier tier)
    {
        var newTier = new Tier(tier.Id, tier.Name);
        newTier.Quotas.AddRange(tier.Quotas);
        return newTier;
    }
}
