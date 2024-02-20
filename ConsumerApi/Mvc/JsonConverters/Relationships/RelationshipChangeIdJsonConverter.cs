using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Relationships.Domain.Ids;

namespace Backbone.ConsumerApi.Mvc.JsonConverters.Relationships;

public class RelationshipChangeIdJsonConverter : JsonConverter<RelationshipChangeId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(RelationshipChangeId);
    }

    public override RelationshipChangeId Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var id = reader.GetString() ?? throw new JsonException("The id cannot be null.");
        try
        {
            return RelationshipChangeId.Parse(id);
        }
        catch (InvalidIdException ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, RelationshipChangeId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.StringValue);
    }
}
