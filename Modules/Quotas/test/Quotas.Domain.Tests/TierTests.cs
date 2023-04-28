using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FluentAssertions;
using Xunit;

namespace Quotas.Domain.Tests
{
    public class TierTests
    {
        [Fact]
        public void Can_create_tier_with_valid_properties()
        {
            var tierId = "TIREYSCQI6XaMygco7Bw";
            var tierName = "my-test-tier";
            var tier = new Tier(tierId, tierName);

            tier.Id.Should().Be("TIREYSCQI6XaMygco7Bw");
            tier.Name.Should().Be(tierName);
        }
    }
}
