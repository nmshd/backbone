using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.OAuthClient;
public class OAuthClientTests
{
    [Fact]
    public void Client_properties_are_updated_with_new_values()
    {
        // Arrange
        var oldTierId = TierId.Generate();
        var oldMaxIdentities = 1;

        var client = new Entities.OAuthClient(string.Empty, string.Empty, oldTierId, SystemTime.UtcNow, oldMaxIdentities);

        var newTierId = TierId.Generate();
        var newMaxIdentities = 2;

        // Act
        var hasChanges = client.Update(newTierId, newMaxIdentities);

        // Assert
        hasChanges.Should().BeTrue();
        client.DefaultTier.Should().Be(newTierId);
        client.MaxIdentities.Should().Be(newMaxIdentities);
    }

    [Fact]
    public void Client_properties_are_not_updated_with_old_values()
    {
        // Arrange
        var tierId = TierId.Generate();
        var maxIdentities = 1;
        var client = new Entities.OAuthClient(string.Empty, string.Empty, tierId, SystemTime.UtcNow, maxIdentities);

        // Act
        var hasChanges = client.Update(tierId, maxIdentities);

        // Assert
        hasChanges.Should().BeFalse();
    }
}
