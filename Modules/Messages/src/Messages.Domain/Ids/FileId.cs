using System.ComponentModel;
using System.Globalization;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.Modules.Messages.Domain.Ids;

[Serializable]
[TypeConverter(typeof(FileIdTypeConverter))]
public record FileId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "FIL";
    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    private FileId(string stringValue) : base(stringValue)
    {
    }

    public static FileId Parse(string stringValue)
    {
        UTILS.Validate(stringValue);

        return new FileId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return UTILS.IsValid(stringValue);
    }

    public static FileId New()
    {
        var stringValue = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
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
