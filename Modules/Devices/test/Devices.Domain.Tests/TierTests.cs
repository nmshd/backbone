using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentAssertions;
using Xunit;

namespace Devices.Domain.Tests;

public class TierTests
{
    [Fact]
    public void Can_create_tier_with_valid_properties()
    {
        var tierName = TierName.Create("my-test-tier").Value;
        var tier = new Tier(tierName);

        tier.Id.Should().NotBeNull();
        tier.Name.Should().Be(tierName);
    }
}
