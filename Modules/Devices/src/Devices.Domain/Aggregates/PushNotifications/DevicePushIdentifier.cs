using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Hashing;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Classes;
using Backbone.BuildingBlocks.Domain;

namespace Backbone.Modules.Devices.Application.Tests.Tests.PushNotifications;

public class DevicePushIdentifier : StronglyTypedId
{
    private DevicePushIdentifier(string stringValue) : base(stringValue)
    {
        Value = GenerateRandomIdentifier();
    }

    public static DevicePushIdentifier Create(string stringValue)
    {
        return new DevicePushIdentifier(stringValue);
    }

    public string Value { get; }

    public static DeviceId New()
    {
        // DevicePushIdentifier
        var deviceIdAsString = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new DeviceId(PREFIX + deviceIdAsString);
    }
}
