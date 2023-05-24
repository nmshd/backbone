using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Quotas.Domain.Tests;

public class IdentityTests
{
    [Fact]
    public void Can_create_identity_with_valid_properties()
    {
        var address = "someAddress";
        var tierId = "TIRsomeTierId1111111";
        var identity = new Identity(address, tierId);

        identity.Address.Should().Be(address);
        identity.TierId.Should().Be(tierId);
    }

    [Fact]
    public void Can_assign_tier_quota_from_definition_to_identity()
    {
        var address = "someAddress";
        var tierId = "TIRsomeTierId1111111";
        var identity = new Identity(address, tierId);

        var metric = new Metric(MetricKey.NumberOfSentMessages, "Number Of Sent Messages");
        var max = 5;
        var period = QuotaPeriod.Month;
        var tierQuotaDefinition = new TierQuotaDefinition(metric, max, period);

        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);
        // no exception thrown signifies the test worked
    }
}
