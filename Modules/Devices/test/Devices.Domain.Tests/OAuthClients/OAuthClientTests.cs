using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;

namespace Backbone.Modules.Devices.Domain.Tests.OAuthClients;

public class OAuthClientTests : AbstractTestsBase
{
    [Fact]
    public void Client_properties_are_updated_with_new_values()
    {
        // Arrange
        var oldTierId = TierId.Generate();
        const int oldMaxIdentities = 1;

        var client = new OAuthClient(string.Empty, string.Empty, oldTierId, SystemTime.UtcNow, oldMaxIdentities);

        var newTierId = TierId.Generate();
        const int newMaxIdentities = 2;

        const int identitiesCount = 0;

        // Act
        client.Update(newTierId, newMaxIdentities, identitiesCount);

        // Assert
        client.DefaultTier.Should().Be(newTierId);
        client.MaxIdentities.Should().Be(newMaxIdentities);
    }

    [Fact]
    public void Client_update_detects_new_values()
    {
        // Arrange
        var oldTierId = TierId.Generate();
        const int oldMaxIdentities = 1;

        var client = new OAuthClient(string.Empty, string.Empty, oldTierId, SystemTime.UtcNow, oldMaxIdentities);

        var newTierId = TierId.Generate();
        const int newMaxIdentities = 2;

        const int identitiesCount = 0;

        // Act
        var hasChanges = client.Update(newTierId, newMaxIdentities, identitiesCount);

        // Assert
        hasChanges.Should().BeTrue();
    }

    [Fact]
    public void Client_update_detects_old_values()
    {
        // Arrange
        var tierId = TierId.Generate();
        const int maxIdentities = 1;
        var client = new OAuthClient(string.Empty, string.Empty, tierId, SystemTime.UtcNow, maxIdentities);

        const int identitiesCount = 0;

        // Act
        var hasChanges = client.Update(tierId, maxIdentities, identitiesCount);

        // Assert
        hasChanges.Should().BeFalse();
    }

    [Fact]
    public void Client_update_detects_that_new_maximum_identities_value_is_lesser_than_identities_count()
    {
        // Arrange
        var oldTierId = TierId.Generate();
        const int oldMaxIdentities = 2;

        var client = new OAuthClient(string.Empty, string.Empty, oldTierId, SystemTime.UtcNow, oldMaxIdentities);

        const int newMaxIdentities = 1;
        const int identitiesCount = 2;

        // Act
        var acting = () => client.Update(oldTierId, newMaxIdentities, identitiesCount);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.device.maxIdentitiesLessThanCurrentIdentities");
    }
}
