using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

namespace Backbone.Modules.Devices.Abstractions;

public static class DeviceRegistrationErrors
{
    public static ApplicationError RegistrationFailed(string message = "") => new(
        "error.platform.validation.device.registrationFailed",
        string.IsNullOrEmpty(message) ? "The registration of the device failed." : message);
}
