using System.ComponentModel;
using System.Globalization;
using Enmeshed.StronglyTypedIds;

namespace Synchronization.Domain.Entities.Sync;

[Serializable]
[TypeConverter(typeof(ExternalEventIdTypeConverter))]
public class ExternalEventId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "SYI";
    private static readonly StronglyTypedIdHelpers Utils = new(PREFIX, DefaultValidChars, MAX_LENGTH);

    private ExternalEventId(string stringValue) : base(stringValue) { }

    public static ExternalEventId Parse(string stringValue)
    {
        if (!IsValid(stringValue))
            throw new InvalidIdException($"'{stringValue}' is not a valid {nameof(ExternalEventId)}.");

        return new ExternalEventId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return Utils.IsValid(stringValue);
    }

    public static ExternalEventId New()
    {
        var challengeIdAsString = StringUtils.Generate(DefaultValidChars, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new ExternalEventId(PREFIX + challengeIdAsString);
    }

    public class ExternalEventIdTypeConverter : TypeConverter
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
