using DeleteChallengesOfIdentitySlice = Backbone.Modules.Challenges.Module.Features.Challenges.DeleteChallengesOfIdentity;
using Backbone.Modules.Challenges.Module.Features.Challenges.DeleteChallengesOfIdentity;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Challenges.Module.Tests.Tests.Challenges.Commands.DeleteChallengesOfIdentity;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeleteChallengesOfIdentitySlice.Command { IdentityAddress = CreateRandomIdentityAddress() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeleteChallengesOfIdentitySlice.Command { IdentityAddress = "invalid-identity-address" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(DeleteChallengesOfIdentitySlice.Command.IdentityAddress));
    }
}
