using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Tokens.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Domain.Tests.TestHelpers;
using Backbone.UnitTestTools.Shouldly.Extensions;

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
        var domainEvent = token.ShouldHaveASingleDomainEvent<TokenCreatedDomainEvent>();
        domainEvent.TokenId.ShouldBe(token.Id);
        domainEvent.CreatedBy.ShouldBe(address);
    }

    [Fact]
    public void Token_can_be_deleted_by_its_owner()
    {
        var identityAddress = CreateRandomIdentityAddress();
        var token = TestData.CreateToken(identityAddress);

        var acting = () => token.EnsureCanBeDeletedBy(identityAddress);

        acting.ShouldNotThrow();
    }

    [Fact]
    public void Token_can_not_be_deleted_by_others()
    {
        var creatorIdentity = CreateRandomIdentityAddress();
        var otherIdentity = CreateRandomIdentityAddress();
        var token = TestData.CreateToken(creatorIdentity);

        var acting = () => token.EnsureCanBeDeletedBy(otherIdentity);

        acting.ShouldThrow<DomainActionForbiddenException>();
    }
}
