using DeleteFilesOfIdentitySlice = Backbone.Modules.Files.Module.Features.Identities.DeleteFilesOfIdentity;
using Backbone.Modules.Files.Module.Features.Identities.DeleteFilesOfIdentity;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Files.Module.Tests.Tests.Identities.Commands.DeleteFilesOfIdentity;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeleteFilesOfIdentitySlice.Command { IdentityAddress = CreateRandomIdentityAddress() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeleteFilesOfIdentitySlice.Command { IdentityAddress = "invalid-identity-address" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(DeleteFilesOfIdentitySlice.Command.IdentityAddress));
    }
}
