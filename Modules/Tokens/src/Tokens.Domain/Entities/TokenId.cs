using System.ComponentModel;
using System.Globalization;
using Enmeshed.StronglyTypedIds;

namespace Tokens.Domain.Entities;

[Serializable]
[TypeConverter(typeof(TokenIdTypeConverter))]
public class TokenId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "TOK";
    private static readonly StronglyTypedIdHelpers Utils = new(PREFIX, DefaultValidChars, MAX_LENGTH);

    private TokenId(string stringValue) : base(stringValue) { }

    public static TokenId Parse(string stringValue)
    {
        Utils.Validate(stringValue);

        return new TokenId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return Utils.IsValid(stringValue);
    }

    public static TokenId New()
    {
        var stringValue = StringUtils.Generate(DefaultValidChars, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new TokenId(PREFIX + stringValue);
    }

    public class TokenIdTypeConverter : TypeConverter
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
