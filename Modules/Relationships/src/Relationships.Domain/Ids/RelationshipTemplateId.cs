using System.ComponentModel;
using System.Globalization;
using Enmeshed.StronglyTypedIds;

namespace Relationships.Domain.Ids;

[Serializable]
[TypeConverter(typeof(RelationshipTemplateIdTypeConverter))]
public class RelationshipTemplateId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "RLT";
    private static readonly StronglyTypedIdHelpers Utils = new(PREFIX, DefaultValidChars, MAX_LENGTH);

    private RelationshipTemplateId(string stringValue) : base(stringValue) { }

    public static RelationshipTemplateId Parse(string stringValue)
    {
        Utils.Validate(stringValue);

        return new RelationshipTemplateId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return Utils.IsValid(stringValue);
    }

    public static RelationshipTemplateId New()
    {
        var stringValue = StringUtils.Generate(DefaultValidChars, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new RelationshipTemplateId(PREFIX + stringValue);
    }

    public class RelationshipTemplateIdTypeConverter : TypeConverter
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
