using Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Tokens.Application.Tests.Tests.Tokens.CreateToken;
public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_Path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(
            new CreateTokenCommand() {Content = [1], ExpiresAt = DateTime.UtcNow.AddDays(1), ForIdentity = TestDataGenerator.CreateRandomIdentityAddress()});

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
            new CreateTokenCommand() { Content = [], ExpiresAt = DateTime.UtcNow.AddDays(1), ForIdentity = TestDataGenerator.CreateRandomIdentityAddress() });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem("Content", "error.platform.validation.invalidPropertyValue", "'Content' must not be empty.");
        validationResult.ShouldHaveValidationErrorForItem("Content", "error.platform.validation.invalidPropertyValue", "'Content' must be between 1 and 10485760 bytes long. You entered 0 bytes.");
    }

    [Fact]
    public void Fails_when_ExpiresAt_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(
            new CreateTokenCommand() { Content = [1], ExpiresAt = DateTime.UtcNow.AddDays(-1), ForIdentity = TestDataGenerator.CreateRandomIdentityAddress() });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem("ExpiresAt", "error.platform.validation.invalidPropertyValue", "'Expires At' must be in the future.");
    }

    [Fact]
    public void Fails_when_ForIdentity_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(
            new CreateTokenCommand() { Content = [1], ExpiresAt = DateTime.UtcNow.AddDays(1), ForIdentity = "some-address" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(CreateTokenCommand.ForIdentity));
    }
}
