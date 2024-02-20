using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.ConsumerApi.Mvc.JsonConverters.Messages;

public class MessageIdJsonConverter : JsonConverter<MessageId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(MessageId);
    }

    public override MessageId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString();

        if (id == null)
            throw new JsonException("The id cannot be null.");

        try
        {
            return MessageId.Parse(id);
        }
        catch (InvalidIdException ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, MessageId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.StringValue);
    }
}
