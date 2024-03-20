using System.ComponentModel;
using System.Globalization;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Classes;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

[Serializable]
[TypeConverter(typeof(RelationshipChangeIdTypeConverter))]
public class RelationshipChangeId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "RCH";
    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    private RelationshipChangeId(string stringValue) : base(stringValue)
    {
    }

    public static RelationshipChangeId Parse(string stringValue)
    {
        UTILS.Validate(stringValue);

        return new RelationshipChangeId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return UTILS.IsValid(stringValue);
    }

    public static RelationshipChangeId New()
    {
        var stringValue = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new RelationshipChangeId(PREFIX + stringValue);
    }

    public class RelationshipChangeIdTypeConverter : TypeConverter
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
