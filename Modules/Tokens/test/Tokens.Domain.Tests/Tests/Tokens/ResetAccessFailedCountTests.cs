using static Backbone.Modules.Tokens.Domain.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Tokens.Domain.Tests.Tests.Tokens;

public class ResetAccessFailedCountTests : AbstractTestsBase
{
    [Fact]
    public void Unlocks_the_token()
    {
        // Arrange
        var token = CreateLockedToken();
        token.IsLocked.ShouldBeTrue();

        // Act
        token.ResetAccessFailedCount();

        // Assert
        token.IsLocked.ShouldBeFalse();
    }
}
