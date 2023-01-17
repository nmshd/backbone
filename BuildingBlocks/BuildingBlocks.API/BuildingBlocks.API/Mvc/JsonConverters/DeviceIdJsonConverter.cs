using System.Text.Json;
using System.Text.Json.Serialization;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.StronglyTypedIds;

namespace Enmeshed.BuildingBlocks.API.Mvc.JsonConverters
{
    public class DeviceIdJsonConverter : JsonConverter<DeviceId?>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DeviceId);
        }

        public override DeviceId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var id = reader.GetString();

            try
            {
                return id == null ? null : DeviceId.Parse(id);
            }
            catch (InvalidIdException ex)
            {
                throw new JsonException(ex.Message);
            }
        }

        public override void Write(Utf8JsonWriter writer, DeviceId? value, JsonSerializerOptions options)
        {
            if (value == null)
                writer.WriteNullValue();
            else
                writer.WriteStringValue(value.StringValue);
        }
    }
}