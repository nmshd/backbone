using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Synchronization.Application.Tests;

public static class TestDataGenerator
{
    public static IdentityAddress CreateRandomIdentityAddress()
    {
        return IdentityAddress.Create(CreateRandomBytes(), "prod.enmeshed.eu");
    }

    public static DeviceId CreateRandomDeviceId()
    {
        return DeviceId.New();
    }

    public static byte[] CreateRandomBytes()
    {
        var random = new Random();
        var bytes = new byte[10];
        random.NextBytes(bytes);
        return bytes;
    }
}
