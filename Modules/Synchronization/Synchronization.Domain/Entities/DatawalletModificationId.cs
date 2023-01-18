using System.ComponentModel;
using System.Globalization;
using Enmeshed.StronglyTypedIds;

namespace Synchronization.Domain.Entities;

[Serializable]
[TypeConverter(typeof(DatawalletModificationIdTypeConverter))]
public class DatawalletModificationId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "DWM";
    private static readonly StronglyTypedIdHelpers Utils = new(PREFIX, DefaultValidChars, MAX_LENGTH);

    private DatawalletModificationId(string stringValue) : base(stringValue) { }

    public static DatawalletModificationId Parse(string stringValue)
    {
        if (!IsValid(stringValue))
            throw new InvalidIdException($"'{stringValue}' is not a valid {nameof(DatawalletModificationId)}.");

        return new DatawalletModificationId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return Utils.IsValid(stringValue);
    }

    public static DatawalletModificationId New()
    {
        var challengeIdAsString = StringUtils.Generate(DefaultValidChars, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new DatawalletModificationId(PREFIX + challengeIdAsString);
    }

    public class DatawalletModificationIdTypeConverter : TypeConverter
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
