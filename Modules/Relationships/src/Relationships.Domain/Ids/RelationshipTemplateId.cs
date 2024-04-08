using System.ComponentModel;
using System.Globalization;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Classes;

namespace Backbone.Modules.Relationships.Domain.Ids;

[Serializable]
[TypeConverter(typeof(RelationshipTemplateIdTypeConverter))]
public class RelationshipTemplateId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "RLT";
    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    private RelationshipTemplateId(string stringValue) : base(stringValue) { }

    public static RelationshipTemplateId Parse(string stringValue)
    {
        UTILS.Validate(stringValue);

        return new RelationshipTemplateId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return UTILS.IsValid(stringValue);
    }

    public static RelationshipTemplateId New()
    {
        var stringValue = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
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
