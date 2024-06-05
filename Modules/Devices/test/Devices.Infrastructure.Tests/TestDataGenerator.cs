using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Infrastructure.Tests;

public static class TestDataGenerator
{
    public static Device CreateDevice(string? language = null)
    {
        return CreateIdentityWithOneDevice(language).Devices.First();
    }

    public static Identity CreateIdentityWithOneDevice(string? deviceCommunicationLanguage = null)
    {
        var identity = new Identity(
            CreateRandomDeviceId(),
            CreateRandomIdentityAddress(),
            CreateRandomBytes(),
            CreateRandomTierId(),
            1
        );

        identity.Devices.Add(new Device(identity, deviceCommunicationLanguage != null ? CommunicationLanguage.Create(deviceCommunicationLanguage).Value : CommunicationLanguage.DEFAULT_LANGUAGE));

        return identity;
    }

    public static TierId CreateRandomTierId()
    {
        return TierId.Generate();
    }
}
