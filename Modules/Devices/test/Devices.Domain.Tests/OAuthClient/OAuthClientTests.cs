using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.OAuthClient;
public class OAuthClientTests
{
    [Fact]
    public void Default_tier_cannot_be_changed_to_same_tier()
    {
        // Arrange
        var tierId = TierId.Generate();
        var client = new Entities.OAuthClient(string.Empty, string.Empty, tierId);

        // Act
        var error = client.ChangeDefaultTier(tierId);

        // Assert
        error.Should().NotBeNull();
        error.Should().BeEquivalentTo(DomainErrors.CannotChangeClientDefaultTier("The Client already uses the provided Default Tier."));
    }
}
