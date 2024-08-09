using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Devices.Commands.DeleteDevice;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new DeleteDeviceCommand { DeviceId = DeviceId.New() });

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_device_id_is_invalid()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new DeleteDeviceCommand { DeviceId = "some-invalid-device-id" });

        validationResult.ShouldHaveValidationErrorFor(x => x.DeviceId);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("The ID is not valid. Check length, prefix and the used characters.");
    }
}
