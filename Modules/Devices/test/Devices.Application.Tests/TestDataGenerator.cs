using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.Tests;

public static class TestDataGenerator
{
    public static IdentityAddress CreateRandomIdentityAddress()
    {
        return IdentityAddress.Create(CreateRandomBytes(), "id1");
    }

    public static Tier CreateRandomTier()
    {
        return new(TierName.Create(TierName.BASIC_DEFAULT_NAME).Value);
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
