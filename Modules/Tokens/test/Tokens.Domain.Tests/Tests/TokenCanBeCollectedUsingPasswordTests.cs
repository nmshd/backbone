using Backbone.Modules.Tokens.Domain.Tests.TestHelpers;

namespace Backbone.Modules.Tokens.Domain.Tests.Tests;

public class TokenCanBeCollectedUsingPasswordTests : AbstractTestsBase
{
    [Fact]
    public void Can_collect_without_a_password_when_no_password_is_defined()
    {
        // Arrange
        var creator = CreateRandomIdentityAddress();
        var collector = CreateRandomIdentityAddress();

        var template = TestData.CreateToken(creator, null);

        // Act
        var result = template.CanBeCollectedUsingPassword(collector, null);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Can_collect_with_a_password_when_no_password_is_defined()
    {
        // Arrange
        var creator = CreateRandomIdentityAddress();
        var collector = CreateRandomIdentityAddress();

        var template = TestData.CreateToken(creator, null);

        // Act
        var result = template.CanBeCollectedUsingPassword(collector, [1]);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Can_collect_with_correct_password()
    {
        // Arrange
        var creator = CreateRandomIdentityAddress();
        var collector = CreateRandomIdentityAddress();

        var template = TestData.CreateToken(creator, null, password: [1]);

        // Act
        var result = template.CanBeCollectedUsingPassword(collector, [1]);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Cannot_collect_with_incorrect_password()
    {
        // Arrange
        var creator = CreateRandomIdentityAddress();
        var collector = CreateRandomIdentityAddress();

        var template = TestData.CreateToken(creator, null, password: [1]);

        // Act
        var result = template.CanBeCollectedUsingPassword(collector, [2]);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Can_collect_as_owner_without_a_password()
    {
        // Arrange
        var creator = CreateRandomIdentityAddress();

        var template = TestData.CreateToken(creator, null, password: [1]);

        // Act
        var result = template.CanBeCollectedUsingPassword(creator, null);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Can_collect_as_anonymous_user_with_correct_password()
    {
        // Arrange
        var creator = CreateRandomIdentityAddress();
        var template = TestData.CreateToken(creator, null, password: [1]);

        // Act
        var result = template.CanBeCollectedUsingPassword(null, [1]);

        // Assert
        result.Should().BeTrue();
    }
}
