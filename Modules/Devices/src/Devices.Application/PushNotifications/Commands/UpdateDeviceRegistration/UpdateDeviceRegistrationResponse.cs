using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;

namespace Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;

public class UpdateDeviceRegistrationResponse
{
    public UpdateDeviceRegistrationResponse(DevicePushIdentifier devicePushIdentifier)
    {
        DevicePushIdentifier = devicePushIdentifier;
    }

    public string DevicePushIdentifier { get; }
}
