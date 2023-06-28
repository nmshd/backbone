using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.UnitTestTools.Extensions;

namespace Backbone.Modules.Quotas.Domain.Tests;
public class QuotaTests
{
    [Fact]
    public void CalculateExhaustion_on_unexhausted_quota_returns_null()
    {
        // Arrange
        var definition = new TierQuotaDefinition(MetricKey.NumberOfFiles, 2, QuotaPeriod.Hour);
        var quota = new TierQuota(definition, "applyTo");

        // Act
        var result = quota.CalculateExhaustion(1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CalculateExhaustion_on_exhausted_quota_returns_DateTime()
    {
        // Arrange
        var definition = new TierQuotaDefinition(MetricKey.NumberOfFiles, 2, QuotaPeriod.Hour);
        var quota = new TierQuota(definition, "applyTo");

        // Act
        var result = quota.CalculateExhaustion(2);

        // Assert
        result.Should().BeEndOfHour();
    }
}
