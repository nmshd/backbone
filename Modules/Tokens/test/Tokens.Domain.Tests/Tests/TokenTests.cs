using Backbone.Modules.Tokens.Domain.DomainEvents;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Domain.Tests.TestHelpers;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Tokens.Domain.Tests.Tests;

public class TokenTests : AbstractTestsBase
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

    [Fact]
    public void Expression_CanBeCollectedBy_null_forIdentity()
    {
        // Arrange
        var token = TestData.CreateToken(createdBy: TestDataGenerator.CreateRandomIdentityAddress(), forIdentity: null);

        // Act
        var result = EvaluateCanBeCollectedByExpression(token, "anyIdentityAddress");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Expression_CanBeCollectedBy_collector_matching_creator()
    {
        // Arrange
        var createdByIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var createdForIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var token = TestData.CreateToken(createdByIdentityAddress, createdForIdentityAddress);

        // Act
        var result = EvaluateCanBeCollectedByExpression(token, createdByIdentityAddress);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Expression_CanBeCollectedBy_collector_matching_forIdentity()
    {
        // Arrange
        var createdForIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var token = TestData.CreateToken(createdBy: TestDataGenerator.CreateRandomIdentityAddress(), createdForIdentityAddress);

        // Act
        var result = EvaluateCanBeCollectedByExpression(token, createdForIdentityAddress);

        // Assert
        result.Should().BeTrue();
    }


    [Fact]
    public void Expression_CanBeCollectedBy_collector_is_third_identity()
    {
        // Arrange
        var token = TestData.CreateToken(createdBy: TestDataGenerator.CreateRandomIdentityAddress(), forIdentity: TestDataGenerator.CreateRandomIdentityAddress());

        // Act
        var result = EvaluateCanBeCollectedByExpression(token, "anotherIdentityAddress");

        // Assert
        result.Should().BeFalse();
    }

    private static bool EvaluateCanBeCollectedByExpression(Token token, string identityAddress)
    {
        var expression = Token.CanBeCollectedBy(identityAddress);
        var result = expression.Compile()(token);
        return result;
    }
}
