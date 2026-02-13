using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.UnitTestTools.FluentValidation;
using Backbone.UnitTestTools.TestDoubles;
using FakeItEasy;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Tokens.Application.Tests.Tests.Tokens.CreateToken;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path_for_authenticated_user_with_optional_parameters()
    {
        // Arrange
        var validator = CreateValidatorForAuthenticatedUser();

        // Act
        var validationResult = validator.TestValidate(
            new CreateTokenCommand
            {
                Content = [1],
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                ForIdentity = "did:e:prod.enmeshed.eu:dids:70cf4f3e6edf6bca33d35f",
                Password = [1, 2, 3]
            });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Happy_path_for_authenticated_user_without_optional_parameters()
    {
        // Arrange
        var validator = CreateValidatorForAuthenticatedUser();

        // Act
        var validationResult = validator.TestValidate(
            new CreateTokenCommand { Content = [0], ExpiresAt = DateTime.UtcNow.AddDays(1) });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Happy_path_for_unauthenticated_user_without_optional_parameters()
    {
        // Arrange
        var validator = CreateValidatorForUnauthenticatedUser();

        // Act
        var validationResult = validator.TestValidate(
            new CreateTokenCommand { ExpiresAt = DateTime.UtcNow.AddMinutes(1) });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Happy_path_for_unauthenticated_user_with_optional_parameters()
    {
        // Arrange
        var validator = CreateValidatorForUnauthenticatedUser();

        // Act
        var validationResult = validator.TestValidate(
            new CreateTokenCommand
            {
                ExpiresAt = DateTime.UtcNow.AddMinutes(1),
                ForIdentity = "did:e:prod.enmeshed.eu:dids:70cf4f3e6edf6bca33d35f",
                Password = [1, 2, 3]
            });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Allows_creation_of_empty_content_by_authenticated_user()
    {
        // Arrange
        var validator = CreateValidatorForAuthenticatedUser();

        // Act
        var validationResult = validator.TestValidate(
            new CreateTokenCommand { Content = null, ExpiresAt = DateTime.UtcNow.AddDays(1), ForIdentity = CreateRandomIdentityAddress() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_for_authenticated_user_when_ExpiresAt_is_in_the_past()
    {
        // Arrange
        var validator = CreateValidatorForAuthenticatedUser();

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
        var validator = CreateValidatorForAuthenticatedUser();

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
        var validator = CreateValidatorForAuthenticatedUser();

        var password = new byte[250];
        new Random().NextBytes(password);

        // Act
        var validationResult = validator.TestValidate(new CreateTokenCommand { Content = [1], ExpiresAt = DateTime.UtcNow.AddDays(1), Password = password });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(nameof(CreateTokenCommand.Password), "error.platform.validation.invalidPropertyValue",
            "'Password' must be between 1 and 200 bytes long. You entered 250 bytes.");
    }

    [Fact]
    public void Fails_for_unauthenticated_user_when_expiry_time_is_too_long()
    {
        // Arrange
        var userContext = A.Fake<IUserContext>();
        A.CallTo(() => userContext.GetAddressOrNull()).Returns(null);
        var validator = CreateValidatorForUnauthenticatedUser();

        // Act
        var validationResult = validator.TestValidate(new CreateTokenCommand { ExpiresAt = DateTime.UtcNow.AddDays(1) });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(nameof(CreateTokenCommand.ExpiresAt), "error.platform.validation.invalidPropertyValue", "'Expires At' must be less than.*");
    }

    [Fact]
    public void Fails_for_unauthenticated_user_when_content_is_not_null()
    {
        // Arrange
        var validator = CreateValidatorForUnauthenticatedUser();

        // Act
        var validationResult = validator.TestValidate(new CreateTokenCommand { Content = [1], ExpiresAt = DateTime.UtcNow.AddMinutes(1) });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(nameof(CreateTokenCommand.Content), "error.platform.validation.invalidPropertyValue", "");
    }

    private static Validator CreateValidatorForAuthenticatedUser()
    {
        return new Validator(UserContextStub.ForAuthenticatedUser());
    }

    private static Validator CreateValidatorForUnauthenticatedUser()
    {
        var userContext = A.Fake<IUserContext>();
        A.CallTo(() => userContext.GetAddressOrNull()).Returns(null);
        return new Validator(userContext);
    }
}
