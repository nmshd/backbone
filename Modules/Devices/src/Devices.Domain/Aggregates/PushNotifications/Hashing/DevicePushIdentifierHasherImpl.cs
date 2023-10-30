namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Hashing;

public class DevicePushIdentifierHasherImpl : IDevicePushIdentifierHasher
{
    public string HashUtf8(string seed)
    {
        return "randomIdentifier";
    }
}
