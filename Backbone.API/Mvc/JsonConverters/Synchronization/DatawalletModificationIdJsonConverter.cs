using System.Text.Json;
using System.Text.Json.Serialization;
using Enmeshed.StronglyTypedIds;
using Synchronization.Domain.Entities;

namespace Synchronization.API.JsonConverters;

public class DatawalletModificationIdJsonConverter : JsonConverter<DatawalletModificationId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DatawalletModificationId);
    }

    public override DatawalletModificationId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString();

        try
        {
            return DatawalletModificationId.Parse(id);
        }
        catch (InvalidIdException ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, DatawalletModificationId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.StringValue);
    }
}
