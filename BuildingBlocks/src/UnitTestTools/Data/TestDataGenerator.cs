using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.UnitTestTools.Data;

public static class TestDataGenerator
{
    public static string GenerateString(int resultLength, char[]? chars = null)
    {
        chars ??= ['A', 'B', 'C'];

        Random random = new();
        return new string(Enumerable.Repeat(chars, resultLength).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static IdentityAddress CreateRandomIdentityAddress()
    {
        return IdentityAddress.Create(CreateRandomBytes(), "prod.enmeshed.eu");
    }

    public static DeviceId CreateRandomDeviceId()
    {
        return DeviceId.New();
    }

    public static byte[] CreateRandomBytes(int length = 10)
    {
        var random = new Random();
        var bytes = new byte[length];
        random.NextBytes(bytes);
        return bytes;
    }

    public static Identity CreateIdentity(IdentityAddress identityAddress)
    {
        return new Identity(
            CreateRandomDeviceId(),
            identityAddress,
            CreateRandomBytes(),
            CreateRandomTierId(),
            1,
            CommunicationLanguage.DEFAULT_LANGUAGE);
    }

    public static TierId CreateRandomTierId()
    {
        return TierId.Generate();
    }
}
