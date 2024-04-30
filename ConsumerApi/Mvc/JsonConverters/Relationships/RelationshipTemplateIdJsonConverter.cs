using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Relationships.Domain.Ids;

namespace Backbone.ConsumerApi.Mvc.JsonConverters.Relationships;

public class RelationshipTemplateIdJsonConverter : JsonConverter<RelationshipTemplateId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(RelationshipTemplateId);
    }

    public override RelationshipTemplateId Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var id = reader.GetString() ?? throw new JsonException("The id cannot be null.");
        try
        {
            return RelationshipTemplateId.Parse(id);
        }
        catch (InvalidIdException ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, RelationshipTemplateId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
