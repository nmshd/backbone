using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;

public class TierQuotaDefinitionTests
{
    [Fact]
    public void Can_create_tier_quota_definition_with_valid_properties()
    {
        // Arrange
        var metricKey = MetricKey.NumberOfSentMessages;
        var max = 5;
        var period = QuotaPeriod.Month;

        // Act
        var tierQuotaDefinition = new TierQuotaDefinition(metricKey, max, period);

        // Assert
        tierQuotaDefinition.Id.Should().NotBeNull();
        tierQuotaDefinition.MetricKey.Should().Be(metricKey);
        tierQuotaDefinition.Max.Should().Be(max);
        tierQuotaDefinition.Period.Should().Be(period);
    }
}
