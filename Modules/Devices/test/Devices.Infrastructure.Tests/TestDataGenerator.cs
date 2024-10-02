using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
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

    public static Identity CreateIdentityWithOneDevice(string? language = null)
    {
        var deviceCommunicationLanguage = language != null ? CommunicationLanguage.Create(language).Value : CommunicationLanguage.DEFAULT_LANGUAGE;

        var identity = new Identity(
            CreateRandomDeviceId(),
            CreateRandomIdentityAddress(),
            CreateRandomBytes(),
            CreateRandomTierId(),
            1,
            deviceCommunicationLanguage
        );

        return identity;
    }

    public static PnsRegistration CreatePnsRegistrationForSse(IdentityAddress? identityAddress = null)
    {
        identityAddress ??= CreateRandomIdentityAddress();
        return new PnsRegistration(identityAddress, CreateRandomDeviceId(), SseHandle.Create(), "", PushEnvironment.Production);
    }

    public static TierId CreateRandomTierId()
    {
        return TierId.Generate();
    }
}
