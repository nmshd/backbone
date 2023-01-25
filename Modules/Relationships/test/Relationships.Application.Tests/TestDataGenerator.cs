using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Relationships.Application.Tests;

public static class TestDataGenerator
{
    public static IdentityAddress CreateRandomAddress()
    {
        return IdentityAddress.Create(CreateRandomBytes(), "id0");
    }

    public static byte[] CreateRandomBytes()
    {
        var random = new Random();
        var bytes = new byte[10];
        random.NextBytes(bytes);

        return bytes;
    }
}
