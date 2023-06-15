using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;
using Backbone.Modules.Quotas.Domain.Tests.Extensions;
using Enmeshed.Tooling;

namespace Backbone.Modules.Quotas.Domain.Tests;
public class QuotaTests
{
    [Fact]
    public void UpdateExhaustion_on_unexhausted_with_newUsage_under_max_quota_keeps_IsExhaustedUntil_null()
    {
        // Arrange
        var definition = new TierQuotaDefinition(Aggregates.Metrics.MetricKey.NumberOfFiles, 50, QuotaPeriod.Hour);
        var quota = new TierQuota(definition, "applyTo");

        // Act
        quota.UpdateExhaustion(20);

        // Assert
        quota.IsExhaustedUntil.Should().BeNull();
    }

    [Fact]
    public void UpdateExhaustion_on_unexhausted_with_newUsage_over_max_quota_sets_IsExhaustedUntil()
    {
        // Arrange
        var currentDate = new DateTime(2023, 01, 01, 13, 45, 00, 000, DateTimeKind.Utc);
        SystemTime.Set(currentDate);

        var definition = new TierQuotaDefinition(Aggregates.Metrics.MetricKey.NumberOfFiles, 50, QuotaPeriod.Hour);
        var quota = new TierQuota(definition, "applyTo");

        // Act
        quota.UpdateExhaustion(50);

        // Assert
        quota.IsExhaustedUntil.Should().NotBeNull();
        quota.IsExhaustedUntil!.Should().Be("2023-01-01T13:59:59.999");
    }
}
