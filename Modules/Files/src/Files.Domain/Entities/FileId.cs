using System.ComponentModel;
using System.Globalization;
using Enmeshed.StronglyTypedIds;

namespace Files.Domain.Entities;

[Serializable]
[TypeConverter(typeof(FileIdTypeConverter))]
public class FileId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "FIL";
    private static readonly StronglyTypedIdHelpers Utils = new(PREFIX, DefaultValidChars, MAX_LENGTH);

    private FileId(string stringValue) : base(stringValue) { }

    public static FileId Parse(string stringValue)
    {
        Utils.Validate(stringValue);

        return new FileId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return Utils.IsValid(stringValue);
    }

    public static FileId New()
    {
        var stringValue = StringUtils.Generate(DefaultValidChars, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new FileId(PREFIX + stringValue);
    }

    public class FileIdTypeConverter : TypeConverter
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
