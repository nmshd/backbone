namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Hashing;

public class DevicePushIdentifierSuffixGeneratorImpl : IDevicePushIdentifierSuffixGenerator
{
    private readonly string _seed;

    public DevicePushIdentifierSuffixGeneratorImpl()
    {
        _seed = "test";
    }

    public string GenerateSuffixUtf8()
    {
        return _seed;
    }
}
