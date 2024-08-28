using System.ComponentModel;
using System.Globalization;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;

[Serializable]
[TypeConverter(typeof(DevicePushIdentifierTypeConverter))]
public record DevicePushIdentifier : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "DPI";
    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    private DevicePushIdentifier(string stringValue) : base(stringValue)
    {
    }

    public static DevicePushIdentifier New()
    {
        var devicePushIdentifierIdAsString = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new DevicePushIdentifier(PREFIX + devicePushIdentifierIdAsString);
    }

    public static DevicePushIdentifier Parse(string stringValue)
    {
        return new DevicePushIdentifier(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return UTILS.IsValid(stringValue);
    }

    public class DevicePushIdentifierTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            var stringValue = value as string;

            return !string.IsNullOrEmpty(stringValue)
                ? Parse(stringValue)
                : null;
        }
    }
}
