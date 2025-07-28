using Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Tokens.Application.Tests.Tests.Tokens.CreateToken;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_Path_with_optional_parameters()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(
            new CreateTokenCommand { Content = [1], ExpiresAt = DateTime.UtcNow.AddDays(1), ForIdentity = "did:e:prod.enmeshed.eu:dids:70cf4f3e6edf6bca33d35f", Password = [1, 2, 3] });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Happy_Path_without_optional_parameters()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(
            new CreateTokenCommand { Content = [1], ExpiresAt = DateTime.UtcNow.AddDays(1) });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_Content_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(
            new CreateTokenCommand { Content = [], ExpiresAt = DateTime.UtcNow.AddDays(1), ForIdentity = CreateRandomIdentityAddress() });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem("Content", "error.platform.validation.invalidPropertyValue", "'Content' must not be empty.");
        validationResult.ShouldHaveValidationErrorForItem("Content", "error.platform.validation.invalidPropertyValue",
            "'Content' must be between 1 and 10485760 bytes long. You entered 0 bytes.");
    }

    [Fact]
    public void Fails_when_ExpiresAt_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(
            new CreateTokenCommand { Content = [1], ExpiresAt = DateTime.UtcNow.AddDays(-1), ForIdentity = CreateRandomIdentityAddress() });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(nameof(Token.ExpiresAt), "error.platform.validation.invalidPropertyValue", "'Expires At' must be in the future.");
    }

    [Fact]
    public void Fails_when_ForIdentity_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(
            new CreateTokenCommand { Content = [1], ExpiresAt = DateTime.UtcNow.AddDays(1), ForIdentity = "some-address" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(Token.ForIdentity));
    }

    [Fact]
    public void Fails_when_Password_is_too_long()
    {
        // Arrange
        var validator = new Validator();

        var password = new byte[250];
        new Random().NextBytes(password);

        // Act
        var validationResult = validator.TestValidate(new CreateTokenCommand { Content = [1], ExpiresAt = DateTime.UtcNow.AddDays(1), Password = password });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(nameof(CreateTokenCommand.Password), "error.platform.validation.invalidPropertyValue",
            "'Password' must be between 1 and 200 bytes long. You entered 250 bytes.");
    }
}
