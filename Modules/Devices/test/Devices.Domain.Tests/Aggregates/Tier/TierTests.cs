using TierEntity = Backbone.Modules.Devices.Domain.Aggregates.Tier.Tier;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;

namespace Backbone.Modules.Devices.Domain.Tests.Aggregates.Tier;

public class TierTests : AbstractTestsBase
{
    [Fact]
    public void Can_create_tier_with_valid_properties()
    {
        var tierName = TierName.Create("my-test-tier").Value;
        var tier = new TierEntity(tierName);

        tier.Id.ShouldNotBeNull();
        tier.Name.ShouldBe(tierName);
        tier.CanBeUsedAsDefaultForClient.ShouldBeTrue();
        tier.CanBeManuallyAssigned.ShouldBeTrue();
    }

    [Fact]
    public void Basic_Tier_cannot_be_deleted()
    {
        // Arrange
        var tier = new TierEntity(TierName.BASIC_DEFAULT_NAME);

        // Act
        var error = tier.CanBeDeleted(clientsCount: 0, identitiesCount: 0);

        // Assert
        error.ShouldNotBeNull();
        error!.Code.ShouldBe("error.platform.validation.device.basicTierCannotBeDeleted");
    }

    [Fact]
    public void Queued_for_deletion_tier_cannot_be_deleted()
    {
        // Act
        var error = TierEntity.QUEUED_FOR_DELETION.CanBeDeleted(clientsCount: 0, identitiesCount: 0);

        // Assert
        error.ShouldNotBeNull();
        error!.Code.ShouldBe("error.platform.validation.device.queuedForDeletionTierCannotBeDeleted");
    }

    [Fact]
    public void Tier_with_related_identities_cannot_be_deleted()
    {
        // Arrange
        var tier = new TierEntity(TierName.Create("tier-name").Value);

        // Act
        var error = tier.CanBeDeleted(clientsCount: 0, identitiesCount: 1);

        // Assert
        error!.Code.ShouldBe("error.platform.validation.device.usedTierCannotBeDeleted");
        error.Message.ShouldContain("Tier is assigned to one or more Identities");
    }

    [Fact]
    public void Tier_with_related_clients_cannot_be_deleted()
    {
        // Arrange
        var tier = new TierEntity(TierName.Create("tier-name").Value);

        // Act
        var error = tier.CanBeDeleted(clientsCount: 1, identitiesCount: 0);

        // Assert
        error!.Code.ShouldBe("error.platform.validation.device.usedTierCannotBeDeleted");
        error.Message.ShouldContain("The Tier is used as the default Tier by one or more clients.");
    }
}
