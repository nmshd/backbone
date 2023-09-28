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
        var result = tier.CanBeDeleted(clientsCount: 0, identitiesCount: 0);

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
        var result = tier.CanBeDeleted(clientsCount: 0, identitiesCount: 1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(DomainErrors.CannotDeleteUsedTier($"The Tier is assigned to one or more Identities. A Tier cannot be deleted if there are Identities assigned to it ({1} found)."));
    }

    [Fact]
    public void Tier_with_related_clients_cannot_be_deleted()
    {
        // Arrange
        var tier = new Tier(TierName.Create("tier-name").Value);

        // Act
        var result = tier.CanBeDeleted(clientsCount: 1, identitiesCount: 0);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(DomainErrors.CannotDeleteUsedTier($"The Tier is used as the default Tier by one or more clients. A Tier cannot be deleted if it is the default Tier of a Client ({1} found)."));
    }
}
