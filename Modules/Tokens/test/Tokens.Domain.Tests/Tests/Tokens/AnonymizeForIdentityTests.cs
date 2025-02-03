using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.UnitTestTools.Extensions;

namespace Backbone.Modules.Tokens.Domain.Tests.Tests.Tokens;

public class TokenAnonymizeForIdentityTests : AbstractTestsBase
{
    private const string DID_DOMAIN_NAME = "localhost";

    [Fact]
    public void Personalized_token_can_be_anonymized()
    {
        // Arrange
        var creatorIdentityAddress = CreateRandomIdentityAddress();
        var forIdentityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        byte[] content = [1, 1, 1, 1, 1, 1, 1, 1];
        var expiresAt = _dateTimeTomorrow;
        var relationshipTemplate = new Token(creatorIdentityAddress, deviceId, content, expiresAt, forIdentityAddress);

        // Act
        relationshipTemplate.AnonymizeForIdentity(DID_DOMAIN_NAME);

        // Assert
        relationshipTemplate.ForIdentity.Should().Be(IdentityAddress.GetAnonymized(DID_DOMAIN_NAME));
    }

    [Fact]
    public void Non_personalized_token_can_not_be_anonymized()
    {
        // Arrange
        var creatorIdentityAddress = CreateRandomIdentityAddress();
        var deviceId = CreateRandomDeviceId();
        byte[] content = [1, 1, 1, 1, 1, 1, 1, 1];
        var expiresAt = _dateTimeTomorrow;
        var relationshipTemplate = new Token(creatorIdentityAddress, deviceId, content, expiresAt);

        // Act
        var acting = () => relationshipTemplate.AnonymizeForIdentity(DID_DOMAIN_NAME);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.token.tokenNotPersonalized");
    }
}
