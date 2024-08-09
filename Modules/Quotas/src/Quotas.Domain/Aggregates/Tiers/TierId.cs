using System.ComponentModel;
using System.Globalization;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

[Serializable]
[TypeConverter(typeof(TierIdTypeConverter))]
public record TierId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "TIR";

    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    public static implicit operator string(TierId id)
    {
        return id.Value;
    }

    private TierId(string value) : base(value)
    {
    }

    public static bool IsValid(string stringValue)
    {
        return UTILS.IsValid(stringValue);
    }

    public static TierId Parse(string stringValue)
    {
        return new TierId(stringValue);
    }

    public class TierIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            var stringValue = value as string;

            return !string.IsNullOrEmpty(stringValue) ? Parse(stringValue) : null;
        }
    }
}
