using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;

public class TierQuotaTests
{
    [Fact]
    public void Can_create_tier_quota_with_valid_properties()
    {
        var metric = new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages");
        var max = 5;
        var period = QuotaPeriod.Month;
        var tierQuotaDefinition = new TierQuotaDefinition(metric, max, period);

        var applyTo = TestDataGenerator.CreateRandomIdentityAddress();

        var tierQuota = new TierQuota(tierQuotaDefinition, applyTo);

        tierQuota.Id.Should().NotBeNull();
        tierQuota.ApplyTo.Should().Be(applyTo);
        tierQuota.Metric.Should().Be(metric);
        tierQuota.Period.Should().Be(period);
        tierQuota.Max.Should().Be(max);
        tierQuota.Weight.Should().Be(2);
    }
}
