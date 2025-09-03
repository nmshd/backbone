using Backbone.Modules.Tokens.Domain.Entities;
using static Backbone.Modules.Tokens.Domain.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Tokens.Domain.Tests.Tests.Tokens;

public class UpdateContentTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path_without_forIdentity_and_without_password()
    {
        // Arrange
        var token = CreateTokenWithoutContent();

        var updatedContent = CreateRandomBytes();
        var activeIdentity = CreateRandomIdentityAddress();
        var activeDevice = CreateRandomDeviceId();

        // Act
        var accessResult = token.UpdateContent(updatedContent, activeIdentity, activeDevice, null);

        // Assert
        accessResult.ShouldBe(UpdateTokenContentResult.ContentUpdated);
        token.Details.Content.ShouldBe(updatedContent);
        token.CreatedBy.ShouldBe(activeIdentity);
        token.CreatedByDevice.ShouldBe(activeDevice);
    }

    [Fact]
    public void Happy_path_with_ForIdentity_and_with_Password()
    {
        // Arrange
        var password = CreateRandomBytes();
        var activeIdentity = CreateRandomIdentityAddress();

        var token = CreateTokenWithoutContent(forIdentity: activeIdentity, password: password);

        var updatedContent = CreateRandomBytes();
        var activeDevice = CreateRandomDeviceId();

        // Act
        var accessResult = token.UpdateContent(updatedContent, activeIdentity, activeDevice, password);

        // Assert
        accessResult.ShouldBe(UpdateTokenContentResult.ContentUpdated);
        token.Details.Content.ShouldBe(updatedContent);
        token.CreatedBy.ShouldBe(activeIdentity);
        token.CreatedByDevice.ShouldBe(activeDevice);
    }

    [Fact]
    public void ForIdentity_does_not_match()
    {
        // Arrange
        var forIdentity = CreateRandomIdentityAddress();
        var token = CreateTokenWithoutContent(forIdentity: forIdentity);

        var updatedContent = CreateRandomBytes();
        var activeIdentity = CreateRandomIdentityAddress();
        var activeDevice = CreateRandomDeviceId();

        // Act
        var accessResult = token.UpdateContent(updatedContent, activeIdentity, activeDevice, null);

        // Assert
        accessResult.ShouldBe(UpdateTokenContentResult.ForIdentityDoesNotMatch);
        token.Details.Content.ShouldBe(null);
        token.CreatedBy.ShouldBe(null);
        token.CreatedByDevice.ShouldBe(null);
    }

    [Fact]
    public void Password_does_not_match()
    {
        // Arrange
        var actualPassword = CreateRandomBytes();
        var token = CreateTokenWithoutContent(password: actualPassword);

        var activeIdentity = CreateRandomIdentityAddress();
        var activeDevice = CreateRandomDeviceId();
        var updatedContent = CreateRandomBytes();
        var passwordUsedForUpdate = CreateRandomBytes();

        // Act
        var accessResult = token.UpdateContent(updatedContent, activeIdentity, activeDevice, passwordUsedForUpdate);

        // Assert
        accessResult.ShouldBe(UpdateTokenContentResult.WrongPassword);
        token.Details.Content.ShouldBe(null);
        token.CreatedBy.ShouldBe(null);
        token.CreatedByDevice.ShouldBe(null);
    }

    [Fact]
    public void Token_is_expired()
    {
        // Arrange
        var token = CreateTokenWithoutContent(expiresAt: DateTime.UtcNow.AddMinutes(-1));

        var activeIdentity = CreateRandomIdentityAddress();
        var activeDevice = CreateRandomDeviceId();
        var updatedContent = CreateRandomBytes();

        // Act
        var accessResult = token.UpdateContent(updatedContent, activeIdentity, activeDevice, null);

        // Assert
        accessResult.ShouldBe(UpdateTokenContentResult.Expired);
        token.Details.Content.ShouldBe(null);
        token.CreatedBy.ShouldBe(null);
        token.CreatedByDevice.ShouldBe(null);
    }

    [Fact]
    public void Content_already_exists()
    {
        // Arrange
        var token = CreateToken();
        var initialContent = token.Details.Content;
        var initialCreatedBy = token.CreatedBy;
        var initialCreatedByDevice = token.CreatedByDevice;

        var activeIdentity = CreateRandomIdentityAddress();
        var activeDevice = CreateRandomDeviceId();
        var updatedContent = CreateRandomBytes();

        // Act
        var accessResult = token.UpdateContent(updatedContent, activeIdentity, activeDevice, null);

        // Assert
        accessResult.ShouldBe(UpdateTokenContentResult.ContentAlreadyExists);
        token.Details.Content.ShouldBe(initialContent);
        token.CreatedBy.ShouldBe(initialCreatedBy);
        token.CreatedByDevice.ShouldBe(initialCreatedByDevice);
    }
}
