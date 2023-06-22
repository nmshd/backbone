using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities;
public class IdentityTests
{
    [Fact]
    public void Identity_with_unexhausted_tier_quota_has_valid_tier_quota()
    {
        // Arrange
        var identity = new Identity("some-dummy-address", new TierId("some-tier-id"));
        var metricKey = Domain.Aggregates.Metrics.MetricKey.FileStorageCapacity;
        var tierQuotaDefinition = new TierQuotaDefinition(metricKey, 1, QuotaPeriod.Hour);

        // Act
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);

        // Assert
        identity.TierQuotas.Should().HaveCount(1);
        identity.TierQuotas.First().MetricKey.Should().Be(metricKey);
        identity.TierQuotas.First().IsExhaustedUntil.Should().BeNull();
    }

    [Fact]
    public void Identity_with_exhausted_tier_quota_has_non_null_IsExhaustedUntil()
    {
        // Arrange
        var identity = new Identity("some-dummy-address", new TierId("some-tier-id"));
        var metricKey = MetricKey.FileStorageCapacity;
        var tierQuotaDefinition = new TierQuotaDefinition(metricKey, 1, QuotaPeriod.Hour);

        // Act
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);
        identity.TierQuotas.First().UpdateExhaustion(2);

        // Assert
        identity.TierQuotas.First().MetricKey.Should().Be(metricKey);
        identity.TierQuotas.First().IsExhaustedUntil.Should().NotBeNull();
        identity.TierQuotas.First().IsExhaustedUntil.Value.Hour.Should().Be(SystemTime.UtcNow.Hour);
        identity.TierQuotas.First().IsExhaustedUntil.Value.Minute.Should().Be(59);
        identity.TierQuotas.First().IsExhaustedUntil.Value.Second.Should().Be(59);
    }

    [Fact]
    public void Identity_with_more_than_one_exhausted_tier_quota_has_correct_metric_status_IsExhaustedUntil()
    {
        // Arrange
        var identity = new Identity("some-dummy-address", new TierId("some-tier-id"));
        var metricKey = MetricKey.FileStorageCapacity;
        var tierQuotaDefinitions = new List<TierQuotaDefinition>() {
            new (metricKey, 1, QuotaPeriod.Hour),
            new (metricKey, 10, QuotaPeriod.Year)
        };

        foreach (var definition in tierQuotaDefinitions)
        {
            identity.AssignTierQuotaFromDefinition(definition);
        }

        foreach (var quota in identity.TierQuotas)
        {
            quota.UpdateExhaustion(5);
        }

        // Act
        identity.UpdateAllMetricStatuses();

        // Assert
        var metricStatus = identity.MetricStatuses.Where(m => m.MetricKey == metricKey).FirstOrDefault();
        identity.MetricStatuses.Should().HaveCount(1);
        metricStatus.IsExhaustedUntil.Value.Day.Should().Be(SystemTime.UtcNow.Day);
        metricStatus.IsExhaustedUntil.Value.Month.Should().Be(SystemTime.UtcNow.Month);
        metricStatus.IsExhaustedUntil.Value.Year.Should().Be(SystemTime.UtcNow.Year);
    }

    [Fact]
    public void Identity_with_more_than_one_tier_quota_with_different_metricKeys_has_correct_metric_status_count()
    {
        // Arrange
        var identity = new Identity("some-dummy-address", new TierId("some-tier-id"));
        var tierQuotaDefinitions = new List<TierQuotaDefinition>() {
            new (MetricKey.NumberOfRelationships, 1, QuotaPeriod.Hour),
            new (MetricKey.NumberOfFiles, 2, QuotaPeriod.Hour),
            new (MetricKey.NumberOfFiles, 15000, QuotaPeriod.Year),
            new (MetricKey.NumberOfSentMessages, 2, QuotaPeriod.Hour)
        };

        // Act
        tierQuotaDefinitions.ForEach(identity.AssignTierQuotaFromDefinition);
        identity.UpdateAllMetricStatuses();

        // Assert
        identity.MetricStatuses.Should().HaveCount(tierQuotaDefinitions.Select(t => t.MetricKey).Distinct().Count());
        foreach (var metricStatus in identity.MetricStatuses)
        {
            metricStatus.IsExhaustedUntil.Should().BeNull();
        }
    }

    [Fact]
    public void Identity_with_more_than_one_tier_quota_exhausted_in_the_past_has_null_MetricStatuses()
    {
        // Arrange
        SystemTime.Set(DateTime.UtcNow.AddHours(-5));

        var identity = new Identity("some-dummy-address", new TierId("some-tier-id"));
        var tierQuotaDefinitions = new List<TierQuotaDefinition>() {
            new (MetricKey.NumberOfRelationships, 1, QuotaPeriod.Hour),
            new (MetricKey.NumberOfFiles, 2, QuotaPeriod.Hour),
            new (MetricKey.NumberOfSentMessages, 2, QuotaPeriod.Hour)
        };


        foreach (var definition in tierQuotaDefinitions)
        {
            identity.AssignTierQuotaFromDefinition(definition);
        }

        foreach (var quota in identity.TierQuotas)
        {
            quota.UpdateExhaustion(5);
        }

        SystemTime.Set(DateTime.UtcNow.AddHours(10));

        // Act
        identity.UpdateAllMetricStatuses();

        // Assert
        identity.MetricStatuses.Should().HaveCount(tierQuotaDefinitions.Select(t => t.MetricKey).Distinct().Count());
        foreach (var metricStatus in identity.MetricStatuses)
        {
            metricStatus.IsExhaustedUntil.Should().BeNull();
        }
    }
}
