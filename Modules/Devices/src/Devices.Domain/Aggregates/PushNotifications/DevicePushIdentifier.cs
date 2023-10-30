using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Hashing;

namespace Backbone.Modules.Devices.Application.Tests.Tests.PushNotifications;

public class DevicePushIdentifier
{
    private DevicePushIdentifier(DeviceId deviceId)
    {
        // todo: deviceId should be hashed?
        Value = deviceId + "-" + GenerateRandomIdentifier();
    }

    public static DevicePushIdentifier Create(DeviceId deviceId)
    {
        return new DevicePushIdentifier(deviceId);
    }

    public string Value { get; }

    private static string GenerateRandomIdentifier()
    {
        return DevicePushIdentifierSuffixGenerator.GenerateSuffixUtf8();
    }
}
