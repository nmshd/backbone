using System.ComponentModel;
using System.Globalization;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

[Serializable]
[TypeConverter(typeof(TierIdTypeConverter))]
public record TierId
{
    public string Value { get; }

    public static implicit operator string(TierId id)
    {
        return id.Value;
    }

    private TierId(string value)
    {
        Value = value;
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
