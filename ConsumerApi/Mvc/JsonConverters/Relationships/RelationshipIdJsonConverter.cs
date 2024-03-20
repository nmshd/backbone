using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.ConsumerApi.Mvc.JsonConverters.Relationships;

public class RelationshipIdJsonConverter : JsonConverter<RelationshipId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(RelationshipId);
    }

    public override RelationshipId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString() ?? throw new JsonException("The id cannot be null.");
        try
        {
            return RelationshipId.Parse(id);
        }
        catch (InvalidIdException ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, RelationshipId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.StringValue);
    }
}
