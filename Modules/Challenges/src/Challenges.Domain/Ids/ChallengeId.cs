using System.ComponentModel;
using System.Globalization;
using Enmeshed.StronglyTypedIds;

namespace Challenges.Domain.Ids;

[Serializable]
[TypeConverter(typeof(ChallengeIdTypeConverter))]
public class ChallengeId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "CHL";
    private static readonly StronglyTypedIdHelpers Utils = new(PREFIX, DefaultValidChars, MAX_LENGTH);

    private ChallengeId(string stringValue) : base(stringValue) { }

    public static ChallengeId Parse(string stringValue)
    {
        Utils.Validate(stringValue);

        return new ChallengeId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return Utils.IsValid(stringValue);
    }

    public static ChallengeId New()
    {
        var stringValue = StringUtils.Generate(DefaultValidChars, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new ChallengeId(PREFIX + stringValue);
    }

    public class ChallengeIdTypeConverter : TypeConverter
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
