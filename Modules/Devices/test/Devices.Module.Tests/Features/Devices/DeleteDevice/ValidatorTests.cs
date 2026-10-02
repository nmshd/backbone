using DeleteDeviceSlice = Backbone.Modules.Devices.Module.Features.Devices.DeleteDevice;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Module.Features.Devices.DeleteDevice;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Module.Tests.Features.Devices.DeleteDevice;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeleteDeviceSlice.Command { DeviceId = DeviceId.New() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_device_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeleteDeviceSlice.Command { DeviceId = "some-invalid-device-id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(DeleteDeviceSlice.Command.DeviceId));
    }
}
