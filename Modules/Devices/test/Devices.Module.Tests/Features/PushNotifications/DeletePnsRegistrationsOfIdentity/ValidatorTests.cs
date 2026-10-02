using DeletePnsRegistrationsOfIdentitySlice = Backbone.Modules.Devices.Module.Features.PushNotifications.DeletePnsRegistrationsOfIdentity;
using Backbone.Modules.Devices.Module.Features.PushNotifications.DeletePnsRegistrationsOfIdentity;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Module.Tests.Features.PushNotifications.DeletePnsRegistrationsOfIdentity;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeletePnsRegistrationsOfIdentitySlice.Command { IdentityAddress = CreateRandomIdentityAddress() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeletePnsRegistrationsOfIdentitySlice.Command { IdentityAddress = "some-invalid-address" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(DeletePnsRegistrationsOfIdentitySlice.Command.IdentityAddress));
    }
}
