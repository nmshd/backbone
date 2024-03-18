using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.ConsumerApi.Mvc.JsonConverters.Tokens;

public class TokenIdJsonConverter : JsonConverter<TokenId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(TokenId);
    }

    public override TokenId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString() ?? throw new JsonException("The id cannot be null.");
        try
        {
            return TokenId.Parse(id);
        }
        catch (InvalidIdException ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, TokenId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.StringValue);
    }
}
