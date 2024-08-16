using Backbone.DevelopmentKit.Identity.ValueObjects;
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

    [Theory]
    [InlineData("", "byIdentity", true)]
    [InlineData("", "anyIdentity", true)]
    [InlineData("forIdentity", "byIdentity", true)]
    [InlineData("forIdentity", "forIdentity", true)]
    [InlineData("forIdentity", "anyIdentity", false)]
    public void Expression_CanBeCollectedBy_Returns_Correct_Result(string forAddress, string collectorAddress, bool expectedResult)
    {
        // Arrange
        var byIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var forIdentity = string.IsNullOrEmpty(forAddress) ? null : TestDataGenerator.CreateRandomIdentityAddress();
        var collectorIdentity = GetCollectorIdentity(collectorAddress, byIdentity, forIdentity);

        var token = TestData.CreateToken(byIdentity, forIdentity);

        // Act
        var result = EvaluateCanBeCollectedByExpression(token, collectorIdentity!);

        // Assert
        result.Should().Be(expectedResult);
    }

    private static IdentityAddress? GetCollectorIdentity(string collectorAddress, IdentityAddress byIdentity, IdentityAddress? forIdentity)
    {
        return collectorAddress switch
        {
            "byIdentity" => byIdentity,
            "forIdentity" => forIdentity,
            _ => TestDataGenerator.CreateRandomIdentityAddress()
        };
    }

    private static bool EvaluateCanBeCollectedByExpression(Token token, IdentityAddress identityAddress)
    {
        var expression = Token.CanBeCollectedBy(identityAddress);
        var result = expression.Compile()(token);
        return result;
    }
}
