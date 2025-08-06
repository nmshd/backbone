using Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteChallengesOfIdentity;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Challenges.Application.Tests.Tests.Challenges.Commands.DeleteChallengesOfIdentity;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeleteChallengesOfIdentityCommand { IdentityAddress = CreateRandomIdentityAddress() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeleteChallengesOfIdentityCommand { IdentityAddress = "invalid-identity-address" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(DeleteChallengesOfIdentityCommand.IdentityAddress));
    }
}
