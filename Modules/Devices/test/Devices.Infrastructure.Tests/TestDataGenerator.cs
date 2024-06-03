using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Infrastructure.Tests;

public static class TestDataGenerator
{
    public static TierId CreateRandomTierId()
    {
        return TierId.Generate();
    }

    public static Device CreateDevice()
    {
        return CreateIdentityWithOneDevice().Devices.First();
    }

    public static Identity CreateIdentityWithOneDevice()
    {
        var identity = new Identity(
            CreateRandomDeviceId(),
            CreateRandomIdentityAddress(),
            CreateRandomBytes(),
            CreateRandomTierId(),
            1);
        identity.Devices.Add(new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE));

        return identity;
    }
}
