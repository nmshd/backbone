using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Classes;

namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;

public class DevicePushIdentifier : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;

    private const string PREFIX = "DPI";

    private DevicePushIdentifier(string stringValue) : base(stringValue) { }

    public static DevicePushIdentifier New()
    {
        var devicePushIdentifierIdAsString = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new DevicePushIdentifier(PREFIX + devicePushIdentifierIdAsString);
    }

    public static DevicePushIdentifier Parse(string stringValue)
    {
        return new DevicePushIdentifier(stringValue);
    }
}
