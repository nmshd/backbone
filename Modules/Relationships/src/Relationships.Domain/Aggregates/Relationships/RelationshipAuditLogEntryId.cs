using System.ComponentModel;
using System.Globalization;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

[Serializable]
[TypeConverter(typeof(RelationshipAuditLogEntryIdTypeConverter))]
public record RelationshipAuditLogEntryId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "RAL";
    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    private RelationshipAuditLogEntryId(string stringValue) : base(stringValue)
    {
    }

    public static RelationshipAuditLogEntryId Parse(string stringValue)
    {
        UTILS.Validate(stringValue);

        return new RelationshipAuditLogEntryId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return UTILS.IsValid(stringValue);
    }

    public static RelationshipAuditLogEntryId New()
    {
        var stringValue = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new RelationshipAuditLogEntryId(PREFIX + stringValue);
    }

    public class RelationshipAuditLogEntryIdTypeConverter : TypeConverter
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
