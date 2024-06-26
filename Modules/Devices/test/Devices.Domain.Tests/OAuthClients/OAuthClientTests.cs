using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Tooling;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using Xunit;

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

        // Act
        client.Update(newTierId, newMaxIdentities);

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

        // Act
        var hasChanges = client.Update(newTierId, newMaxIdentities);

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

        // Act
        var hasChanges = client.Update(tierId, maxIdentities);

        // Assert
        hasChanges.Should().BeFalse();
    }
}
