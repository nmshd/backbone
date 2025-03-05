using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Tokens.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Domain.Tests.TestHelpers;

namespace Backbone.Modules.Tokens.Domain.Tests.Tests.Tokens;

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
}
