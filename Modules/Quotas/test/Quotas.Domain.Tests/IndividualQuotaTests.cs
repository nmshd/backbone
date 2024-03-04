using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;

public class IndividualQuotaTests
{
    [Fact]
    public void Creates_individual_quota_with_valid_properties()
    {
        // Arrange
        var metricKey = MetricKey.NumberOfSentMessages;
        var max = 10;
        var period = QuotaPeriod.Month;

        // Act
        var individualQuota = new IndividualQuota(metricKey, max, period, "applyTo");

        // Assert
        individualQuota.MetricKey.Should().Be(metricKey);
        individualQuota.Period.Should().Be(period);
        individualQuota.Max.Should().Be(max);
        individualQuota.Weight.Should().Be(2);
    }
}
