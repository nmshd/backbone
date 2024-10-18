using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.UnitTestTools.Data;

public static class TestDataGenerator
{
    public static string CreateRandomString(int resultLength, char[]? chars = null)
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
}
