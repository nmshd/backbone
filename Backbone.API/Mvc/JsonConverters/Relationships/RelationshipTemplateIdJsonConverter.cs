using System.Text.Json;
using System.Text.Json.Serialization;
using Enmeshed.StronglyTypedIds;
using Relationships.Domain.Ids;

namespace Backbone.API.Mvc.JsonConverters.Relationships;

public class RelationshipTemplateIdJsonConverter : JsonConverter<RelationshipTemplateId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(RelationshipTemplateId);
    }

    public override RelationshipTemplateId Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var id = reader.GetString();

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
        writer.WriteStringValue(value.StringValue);
    }
}