using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;

public class IndividualQuotaTests : AbstractTestsBase
{
    [Fact]
    public void Creates_individual_quota_with_valid_properties()
    {
        // Arrange
        var metricKey = MetricKey.NUMBER_OF_SENT_MESSAGES;
        const int max = 10;
        const QuotaPeriod period = QuotaPeriod.Month;

        // Act
        var individualQuota = new IndividualQuota(metricKey, max, period, "applyTo");

        // Assert
        individualQuota.MetricKey.Should().Be(metricKey);
        individualQuota.Period.Should().Be(period);
        individualQuota.Max.Should().Be(max);
        individualQuota.Weight.Should().Be(2);
    }
}
