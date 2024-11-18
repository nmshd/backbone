using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.DomainEvents;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Domain.Tests.TestHelpers;

namespace Backbone.Modules.Tokens.Domain.Tests.Tests;

public class TokenTests : AbstractTestsBase
{
    [Fact]
    public void Raises_TemplateCreatedDomainEvent_on_creation()
    {
        // Arrange
        var address = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        var expiresAt = DateTime.UtcNow;
        byte[] content = [1, 1, 1, 1, 1, 1, 1, 1];

        // Act
        var token = new Token(address, deviceId, content, expiresAt);

        // Assert
        var domainEvent = token.Should().HaveASingleDomainEvent<TokenCreatedDomainEvent>();
        domainEvent.TokenId.Should().Be(token.Id);
        domainEvent.CreatedBy.Should().Be(address);
    }

    private const string I1 = "did:e:prod.enmeshed.eu:dids:70cf4f3e6edf6bca33d35f";
    private const string I2 = "did:e:prod.enmeshed.eu:dids:cdef0d1a1f2545703f40ca";
    private const string I3 = "did:e:prod.enmeshed.eu:dids:998351a946063fa1c28e04";

    [Theory]
    [InlineData(I1, null, null, true)] // anonymous user can collect if no forIdentity
    [InlineData(I1, null, I1, true)] // creator can collect if no forIdentity 
    [InlineData(I1, null, I3, true)] // third party can collect if no forIdentity
    [InlineData(I1, I2, null, false)] // anonymous user can't collect if forIdentity
    [InlineData(I1, I2, I1, true)] // creator can collect if forIdentity
    [InlineData(I1, I2, I2, true)] // forIdentity can collect if forIdentity
    [InlineData(I1, I2, I3, false)] // third party can't collect if forIdentity
    [InlineData(I1, I1, I1, true)] // creator can collect if it is also forIdentity
    [InlineData(I1, I1, I3, false)] // third party can't collect if creator is also forIdentity
    public void Expression_CanBeCollectedBy_Returns_Correct_Result(string creator, string? forIdentity, string? collector, bool expectedResult)
    {
        var creatorAddress = IdentityAddress.ParseUnsafe(creator);
        var forIdentityAddress = forIdentity == null ? null : IdentityAddress.ParseUnsafe(forIdentity);
        var collectorAddress = collector == null ? null : IdentityAddress.ParseUnsafe(collector);

        // Arrange
        var token = TestData.CreateToken(creatorAddress, forIdentityAddress);

        // Act
        var result = EvaluateCanBeCollectedByExpression(token, collectorAddress);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void Token_can_be_deleted_by_its_owner()
    {
        var identityAddress = CreateRandomIdentityAddress();
        var token = TestData.CreateToken(identityAddress, null);

        var acting = () => token.EnsureCanBeDeletedBy(identityAddress);

        acting.Should().NotThrow();
    }

    [Fact]
    public void Token_can_not_be_deleted_by_others()
    {
        var creatorIdentity = CreateRandomIdentityAddress();
        var otherIdentity = CreateRandomIdentityAddress();
        var token = TestData.CreateToken(creatorIdentity, null);

        var acting = () => token.EnsureCanBeDeletedBy(otherIdentity);

        acting.Should().Throw<DomainActionForbiddenException>();
    }

    private static bool EvaluateCanBeCollectedByExpression(Token token, IdentityAddress? identityAddress)
    {
        var expression = Token.CanBeCollectedBy(identityAddress);
        var result = expression.Compile()(token);
        return result;
    }
}
