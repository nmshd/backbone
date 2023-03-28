using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Domain.Entities;
using Enmeshed.BuildingBlocks.Domain;

namespace Backbone.API.Mvc.JsonConverters.Synchronization;

public class DatawalletIdJsonConverter : JsonConverter<DatawalletId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DatawalletId);
    }

    public override DatawalletId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString();

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
