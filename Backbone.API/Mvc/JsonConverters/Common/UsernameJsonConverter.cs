using System.Text.Json;
using System.Text.Json.Serialization;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.BuildingBlocks.Domain;

namespace Backbone.API.Mvc.JsonConverters.Common;

public class UsernameJsonConverter : JsonConverter<Username?>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Username);
    }

    public override Username? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString();

        try
        {
            return id == null ? null : Username.Parse(id);
        }
        catch (InvalidIdException ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, Username? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.StringValue);
    }
}