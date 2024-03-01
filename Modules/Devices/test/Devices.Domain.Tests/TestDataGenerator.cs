using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Domain.Tests;

public static class TestDataGenerator
{
    public static TierId CreateRandomTierId()
    {
        return TierId.Generate();
    }

    public static Identity CreateIdentity()
    {
        return new Identity(
            CreateRandomDeviceId(),
            CreateRandomIdentityAddress(),
            CreateRandomBytes(),
            CreateRandomTierId(),
            1);
    }

    public static string GenerateString(int resultLength, char[]? chars = null)
    {
        chars ??= ['A', 'B', 'C'];

        Random random = new();
        return new string(Enumerable.Repeat(chars, resultLength).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
