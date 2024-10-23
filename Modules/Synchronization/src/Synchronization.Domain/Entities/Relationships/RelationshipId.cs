using System.ComponentModel;
using System.Globalization;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.Modules.Synchronization.Domain.Entities.Relationships;

public record RelationshipId(string Value) : StronglyTypedId(Value)
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "REL";
    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    public static RelationshipId Parse(string stringValue)
    {
        if (!IsValid(stringValue))
            throw new InvalidIdException($"'{stringValue}' is not a valid {nameof(RelationshipId)}.");

        return new RelationshipId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return UTILS.IsValid(stringValue);
    }

    public static RelationshipId New()
    {
        var challengeIdAsString = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new RelationshipId(PREFIX + challengeIdAsString);
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
