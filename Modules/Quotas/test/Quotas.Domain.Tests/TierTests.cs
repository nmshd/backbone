using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;

public class TierTests
{
    [Fact]
    public void Can_create_tier_with_valid_properties()
    {
        var tierId = "TIREYSCQI6XaMygco7Bw";
        var tierName = "my-test-tier";
        var tier = new Tier(tierId, tierName);

        tier.Id.Should().Be(tierId);
        tier.Name.Should().Be(tierName);
    }

    [Fact]
    public void Can_create_quota_on_tier()
    {
        var tierId = "TIREYSCQI6XaMygco7Bw";
        var tierName = "my-test-tier";
        var tier = new Tier(tierId, tierName);

        var metric = new Metric();
        var max = 5;
        var period = QuotaPeriod.Month;
        tier.CreateQuota(metric, max, period);

        tier.Quotas.Should().HaveCount(1);
    }
}
