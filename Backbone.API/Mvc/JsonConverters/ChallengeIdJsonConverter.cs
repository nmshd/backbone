using System.Text.Json;
using System.Text.Json.Serialization;
using Challenges.Domain.Ids;
using Enmeshed.StronglyTypedIds;

namespace Backbone.API.Mvc.JsonConverters;

public class ChallengeIdJsonConverter : JsonConverter<ChallengeId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ChallengeId);
    }

    public override ChallengeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString();

        try
        {
            return ChallengeId.Parse(id);
        }
        catch (InvalidIdException ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, ChallengeId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.StringValue);
    }
}