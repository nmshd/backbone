using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

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
        individualQuota.MetricKey.ShouldBe(metricKey);
        individualQuota.Period.ShouldBe(period);
        individualQuota.Max.ShouldBe(max);
        individualQuota.Weight.ShouldBe(2);
    }
}
