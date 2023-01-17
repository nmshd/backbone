using System.Text.Json;
using System.Text.Json.Serialization;

namespace Enmeshed.BuildingBlocks.API.Mvc.JsonConverters
{
    public class UtcDateTimeConverter : JsonConverter<DateTime>
    {
        internal const string DEFAULT_FORMAT = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffZ";
        private readonly string _format;

        public UtcDateTimeConverter(string format = DEFAULT_FORMAT)
        {
            _format = format;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();

            if (stringValue == null)
                throw new Exception("Value cannot be null");

            return DateTime.Parse(stringValue);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString(_format));
        }
    }

    public class NullableUtcDateTimeConverter : JsonConverter<DateTime?>
    {
        private readonly string _format;

        public NullableUtcDateTimeConverter(string format = UtcDateTimeConverter.DEFAULT_FORMAT)
        {
            _format = format;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime?);
        }

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            return stringValue == null ? null : DateTime.Parse(stringValue);
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
                writer.WriteNullValue();
            else
                writer.WriteStringValue(value.Value.ToUniversalTime().ToString(_format));
        }
    }
}