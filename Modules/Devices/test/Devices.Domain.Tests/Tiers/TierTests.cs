using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Tiers;

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

    [Fact]
    public void Basic_Tier_cannot_be_deleted()
    {
        // Arrange
        var tier = new Tier(TierName.BASIC_DEFAULT_NAME);

        // Act
        var error = tier.CanBeDeleted(clientsCount: 0, identitiesCount: 0);

        // Assert
        error.Should().NotBeNull();
        error!.Code.Should().Be("error.platform.validation.device.basicTierCannotBeDeleted");
    }

    [Fact]
    public void Queued_for_deletion_tier_cannot_be_deleted()
    {
        // Act
        var error = Tier.QUEUED_FOR_DELETION.CanBeDeleted(clientsCount: 0, identitiesCount: 0);

        // Assert
        error.Should().NotBeNull();
        error!.Code.Should().Be("error.platform.validation.device.queuedForDeletionTierCannotBeDeleted");
    }

    [Fact]
    public void Tier_with_related_identities_cannot_be_deleted()
    {
        // Arrange
        var tier = new Tier(TierName.Create("tier-name").Value);

        // Act
        var error = tier.CanBeDeleted(clientsCount: 0, identitiesCount: 1);

        // Assert
        error!.Code.Should().Be("error.platform.validation.device.usedTierCannotBeDeleted");
        error.Message.Should().Contain("Tier is assigned to one or more Identities");
    }

    [Fact]
    public void Tier_with_related_clients_cannot_be_deleted()
    {
        // Arrange
        var tier = new Tier(TierName.Create("tier-name").Value);

        // Act
        var error = tier.CanBeDeleted(clientsCount: 1, identitiesCount: 0);

        // Assert
        error!.Code.Should().Be("error.platform.validation.device.usedTierCannotBeDeleted");
        error.Message.Should().Contain("The Tier is used as the default Tier by one or more clients.");
    }
}
