using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Tooling;
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
        var client = new Entities.OAuthClient(string.Empty, string.Empty, tierId, SystemTime.UtcNow, 1);

        // Act
        var error = client.ChangeDefaultTier(tierId);

        // Assert
        error.Should().BeEquivalentTo(DomainErrors.CannotChangeClientDefaultTier("The Client already uses the provided Default Tier."));
    }

    [Fact]
    public void Maximum_number_of_identities_cannot_be_changed_to_same_number()
    {
        // Arrange
        var tierId = TierId.Generate();
        var client = new Entities.OAuthClient(string.Empty, string.Empty, tierId, SystemTime.UtcNow, 1);

        // Act
        var error = client.ChangeDefaultTier(tierId);

        // Assert
        error.Should().BeEquivalentTo(DomainErrors.CannotChangeClientDefaultTier("The Client already uses the provided Default Tier."));
    }
}
