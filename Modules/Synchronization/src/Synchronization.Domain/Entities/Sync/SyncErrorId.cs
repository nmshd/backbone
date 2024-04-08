using System.ComponentModel;
using System.Globalization;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Classes;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

[Serializable]
[TypeConverter(typeof(SyncErrorIdTypeConverter))]
public class SyncErrorId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "SYE";
    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    private SyncErrorId(string stringValue) : base(stringValue) { }

    public static SyncErrorId Parse(string stringValue)
    {
        if (!IsValid(stringValue))
            throw new InvalidIdException($"'{stringValue}' is not a valid {nameof(SyncErrorId)}.");

        return new SyncErrorId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return UTILS.IsValid(stringValue);
    }

    public static SyncErrorId New()
    {
        var challengeIdAsString = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new SyncErrorId(PREFIX + challengeIdAsString);
    }

    public class SyncErrorIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            var stringValue = value as string;

            return !string.IsNullOrEmpty(stringValue) ? Parse(stringValue) : base.ConvertFrom(context, culture, value);
        }
    }
}
