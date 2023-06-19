using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;
using Backbone.Modules.Quotas.Domain.Tests.Extensions;
using Enmeshed.Tooling;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Tests;
public class QuotaTests
{
    [Fact]
    public void UpdateExhaustion_on_unexhausted_with_newUsage_under_max_quota_keeps_IsExhaustedUntil_null()
    {
        // Arrange
        var definition = new TierQuotaDefinition(MetricKey.NumberOfFiles, 2, QuotaPeriod.Hour);
        var quota = new TierQuota(definition, "applyTo");

        // Act
        quota.UpdateExhaustion(1);

        // Assert
        quota.IsExhaustedUntil.Should().BeNull();
    }

    [Fact]
    public void UpdateExhaustion_on_unexhausted_with_newUsage_over_max_quota_sets_IsExhaustedUntil()
    {
        // Arrange
        SystemTime.Set(new DateTime(2023, 01, 01, 13, 45, 00, 000, DateTimeKind.Utc));

        var definition = new TierQuotaDefinition(MetricKey.NumberOfFiles, 2, QuotaPeriod.Hour);
        var quota = new TierQuota(definition, "applyTo");

        // Act
        quota.UpdateExhaustion(2);

        // Assert
        quota.IsExhaustedUntil.Should().Be("2023-01-01T13:59:59.999");
    }

    [Fact]
    public void UpdateExhaustion_on_exhausted_with_newUsage_under_max_quota_updates_IsExhaustedUntil_to_null()
    {
        // Arrange
        var definition = new TierQuotaDefinition(MetricKey.NumberOfFiles, 2, QuotaPeriod.Hour);
        var quota = new TierQuota(definition, "applyTo");

        // Act
        quota.UpdateExhaustion(3);
        quota.UpdateExhaustion(1);

        // Assert
        quota.IsExhaustedUntil.Should().BeNull();
    }
}
