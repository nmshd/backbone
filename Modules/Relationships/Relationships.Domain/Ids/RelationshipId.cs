using System.ComponentModel;
using System.Globalization;
using Enmeshed.StronglyTypedIds;

namespace Relationships.Domain.Ids;

[Serializable]
[TypeConverter(typeof(RelationshipIdTypeConverter))]
public class RelationshipId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "REL";
    private static readonly StronglyTypedIdHelpers Utils = new(PREFIX, DefaultValidChars, MAX_LENGTH);

    private RelationshipId(string stringValue) : base(stringValue) { }

    public static RelationshipId Parse(string stringValue)
    {
        Utils.Validate(stringValue);

        return new RelationshipId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return Utils.IsValid(stringValue);
    }

    public static RelationshipId New()
    {
        var stringValue = StringUtils.Generate(DefaultValidChars, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new RelationshipId(PREFIX + stringValue);
    }

    public class RelationshipIdTypeConverter : TypeConverter
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
