using System.ComponentModel;
using System.Globalization;
using Enmeshed.StronglyTypedIds;

namespace Enmeshed.DevelopmentKit.Identity.ValueObjects
{
    [Serializable]
    [TypeConverter(typeof(DeviceIdTypeConverter))]
    public class DeviceId : StronglyTypedId
    {
        public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;

        private const string PREFIX = "DVC";

        private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DefaultValidChars, MAX_LENGTH);


        private DeviceId(string stringValue) : base(stringValue)
        {
        }

        public static DeviceId Parse(string stringValue)
        {
            UTILS.Validate(stringValue);

            return new DeviceId(stringValue);
        }

        public static bool IsValid(string stringValue)
        {
            return UTILS.IsValid(stringValue);
        }

        public static DeviceId New()
        {
            var deviceIdAsString = StringUtils.Generate(DefaultValidChars, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
            return new DeviceId(PREFIX + deviceIdAsString);
        }

        public class DeviceIdTypeConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                var stringValue = value as string;

                return !string.IsNullOrEmpty(stringValue)
                    ? Parse(stringValue)
                    : null;
            }
        }
    }
}