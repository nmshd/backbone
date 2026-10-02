using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.UpdateDeviceRegistration;

public class Response
{
    public Response(DevicePushIdentifier devicePushIdentifier)
    {
        DevicePushIdentifier = devicePushIdentifier;
    }

    public string DevicePushIdentifier { get; }
}
