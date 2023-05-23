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
        var metric = new Metric();
        var max = 5;
        var period = QuotaPeriod.Month;
        var tierQuotaDefinition = new TierQuotaDefinition(metric, max, period);

        tierQuotaDefinition.Id.Should().NotBeNull();
        tierQuotaDefinition.Metric.Should().Be(metric);
        tierQuotaDefinition.Max.Should().Be(max);
        tierQuotaDefinition.Period.Should().Be(period);
    }
}
