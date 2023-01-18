using System.Text.Json;
using System.Text.Json.Serialization;
using Enmeshed.StronglyTypedIds;
using Tokens.Domain.Entities;

namespace Tokens.API.JsonConverters;

public class TokenIdJsonConverter : JsonConverter<TokenId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(TokenId);
    }

    public override TokenId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString();

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
