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
}
