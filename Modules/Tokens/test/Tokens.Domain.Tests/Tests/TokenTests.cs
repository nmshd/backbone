using Backbone.Modules.Tokens.Domain.DomainEvents;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Tokens.Domain.Tests.Tests;

public class TokenTests
{
    [Fact]
    public void Raises_TemplateCreatedDomainEvent_on_creation()
    {
        // Arrange
        var address = TestDataGenerator.CreateRandomIdentityAddress();
        var deviceId = TestDataGenerator.CreateRandomDeviceId();
        var expiresAt = DateTime.UtcNow;
        byte[] content = [1, 1, 1, 1, 1, 1, 1, 1];

        // Act
        var token = new Token(address, deviceId, content, expiresAt);

        // Assert
        var domainEvent = token.Should().HaveASingleDomainEvent<TokenCreatedDomainEvent>();
        domainEvent.TokenId.Should().Be(token.Id);
        domainEvent.CreatedBy.Should().Be(address);
    }
}
