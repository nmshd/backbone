using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.ConsumerApi.Mvc.JsonConverters.Synchronization;

public class DatawalletIdJsonConverter : JsonConverter<DatawalletId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DatawalletId);
    }

    public override DatawalletId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString();

        if (id == null)
            throw new JsonException("The id cannot be null.");

        try
        {
            return DatawalletId.Parse(id);
        }
        catch (InvalidIdException ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, DatawalletId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.StringValue);
    }
}
