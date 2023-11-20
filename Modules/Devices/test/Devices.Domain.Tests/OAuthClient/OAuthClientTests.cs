﻿using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.OAuthClient;
public class OAuthClientTests
{
    [Fact]
    public void Client_properties_cannot_be_changed_to_the_same_properties()
    {
        // Arrange
        var tierId = TierId.Generate();
        var client = new Entities.OAuthClient(string.Empty, string.Empty, tierId, SystemTime.UtcNow, 1);

        // Act
        var error = client.Update(tierId, 1);

        // Assert
        error.Should().BeEquivalentTo(DomainErrors.CannotUpdateClient("No properties were changed for the Client."));
    }
}
