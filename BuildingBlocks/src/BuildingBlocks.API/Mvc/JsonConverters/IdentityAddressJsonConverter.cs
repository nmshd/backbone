using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.BuildingBlocks.API.Mvc.JsonConverters;

public class IdentityAddressJsonConverter : JsonConverter<IdentityAddress?>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(IdentityAddress);
    }

    public override IdentityAddress? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var id = reader.GetString();

        try
        {
            return id == null ? null : IdentityAddress.Parse(id);
        }
        catch (InvalidIdException ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, IdentityAddress? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value);
    }
}
