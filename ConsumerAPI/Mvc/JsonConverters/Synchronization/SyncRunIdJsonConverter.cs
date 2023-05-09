using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Enmeshed.BuildingBlocks.Domain;

namespace ConsumerAPI.Mvc.JsonConverters.Synchronization;

public class SyncRunIdJsonConverter : JsonConverter<SyncRunId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(SyncRunId);
    }

    public override SyncRunId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString();

        try
        {
            return SyncRunId.Parse(id);
        }
        catch (InvalidIdException ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, SyncRunId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.StringValue);
    }
}
