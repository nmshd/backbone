using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Tiers;
public class CanBeDeletedTests
{
    [Fact]
    public void Basic_Tier_cannot_be_deleted()
    {
        // Arrange
        var tier = new Tier(TierName.BASIC_DEFAULT_NAME);

        // Act
        var result = tier.CanBeDeleted(0);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(DomainErrors.CannotDeleteBasicTier());
    }

    [Fact]
    public void Tier_with_related_identities_cannot_be_deleted()
    {
        // Arrange
        var tier = new Tier(TierName.Create("tier-name").Value);

        // Act
        var result = tier.CanBeDeleted(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(DomainErrors.CannotDeleteUsedTier(1));
    }
}
