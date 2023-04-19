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
            var tierId = "1";
            var tierName = "name";
            var tier = new Tier(tierId, tierName);

            tier.Id.Should().NotBeNull();
            tier.Name.Should().Be(tierName);
            
        }
    }
}
