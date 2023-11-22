using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Application.Tests;

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
}
