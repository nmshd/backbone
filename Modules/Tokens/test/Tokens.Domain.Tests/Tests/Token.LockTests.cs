using Backbone.Modules.Tokens.Domain.DomainEvents;
using Backbone.Modules.Tokens.Domain.Tests.TestHelpers;

namespace Backbone.Modules.Tokens.Domain.Tests.Tests;

public class TokenLockTests : AbstractTestsBase
{
    [Fact]
    public void Raises_domain_event()
    {
        // Arrange
        var token = TestData.CreateToken(CreateRandomIdentityAddress(), null, [1, 1, 1, 1, 1, 1, 1, 1]);
        token.ClearDomainEvents();

        for (var i = 0; i < 99; i++)
            token.IncrementAccessFailedCount();

        // Act
        token.IncrementAccessFailedCount();

        // Assert
        token.IsLocked.Should().BeTrue();
        token.DomainEvents.Should().HaveCount(1);
        token.DomainEvents[0].Should().BeOfType<TokenLockedDomainEvent>();
    }
}
