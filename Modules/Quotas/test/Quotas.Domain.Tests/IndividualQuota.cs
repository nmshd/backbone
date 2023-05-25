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
        var metric = new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages");
        var max = 10;
        var period = QuotaPeriod.Month;
        var individualQuota = new IndividualQuota(metric, max, period);

        individualQuota.Metric.Should().Be(metric);
        individualQuota.Period.Should().Be(period);
        individualQuota.Max.Should().Be(max);
        individualQuota.Weight.Should().Be(2);
    }
}
