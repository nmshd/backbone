namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Hashing;

public class DevicePushIdentifierSuffixGeneratorImpl : IDevicePushIdentifierSuffixGenerator
{
    public string GenerateSuffixUtf8(string seed)
    {
        return seed;
    }
}
