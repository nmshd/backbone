using System.ComponentModel;
using System.Globalization;
using Enmeshed.StronglyTypedIds;

namespace Messages.Domain.Ids;

[Serializable]
[TypeConverter(typeof(MessageIdTypeConverter))]
public class MessageId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "MSG";
    private static readonly StronglyTypedIdHelpers Utils = new(PREFIX, DefaultValidChars, MAX_LENGTH);

    private MessageId(string stringValue) : base(stringValue) { }

    public static MessageId Parse(string stringValue)
    {
        Utils.Validate(stringValue);

        return new MessageId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return Utils.IsValid(stringValue);
    }

    public static MessageId New()
    {
        var stringValue = StringUtils.Generate(DefaultValidChars, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new MessageId(PREFIX + stringValue);
    }

    public class MessageIdTypeConverter : TypeConverter
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
