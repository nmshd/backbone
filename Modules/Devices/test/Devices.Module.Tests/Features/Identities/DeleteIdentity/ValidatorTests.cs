using Backbone.Modules.Devices.Module.Features.Identities.DeleteIdentity;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using DeleteIdentitySlice = Backbone.Modules.Devices.Module.Features.Identities.DeleteIdentity;

namespace Backbone.Modules.Devices.Module.Tests.Features.Identities.DeleteIdentity;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeleteIdentitySlice.Command { IdentityAddress = CreateRandomIdentityAddress() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeleteIdentitySlice.Command { IdentityAddress = "invalid-identity-address" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(DeleteIdentitySlice.Command.IdentityAddress));
    }
}
