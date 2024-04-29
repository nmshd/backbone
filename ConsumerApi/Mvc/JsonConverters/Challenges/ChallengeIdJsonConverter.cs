using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Challenges.Domain.Ids;

namespace Backbone.ConsumerApi.Mvc.JsonConverters.Challenges;

public class ChallengeIdJsonConverter : JsonConverter<ChallengeId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ChallengeId);
    }

    public override ChallengeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString() ?? throw new JsonException("The id cannot be null.");
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
        writer.WriteStringValue(value);
    }
}
