using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Classes;
using Backbone.BuildingBlocks.Domain;

namespace Backbone.Modules.Devices.Application.Tests.Tests.PushNotifications;

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
}
