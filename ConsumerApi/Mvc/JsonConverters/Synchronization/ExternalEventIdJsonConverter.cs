using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Enmeshed.BuildingBlocks.Domain;

namespace ConsumerApi.Mvc.JsonConverters.Synchronization;

public class ExternalEventIdJsonConverter : JsonConverter<ExternalEventId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ExternalEventId);
    }

    public override ExternalEventId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString();

        try
        {
            return ExternalEventId.Parse(id);
        }
        catch (InvalidIdException ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, ExternalEventId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.StringValue);
    }
}
