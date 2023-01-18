using System.Text.Json;
using System.Text.Json.Serialization;
using Enmeshed.StronglyTypedIds;
using Messages.Domain.Ids;

namespace Backbone.API.Mvc.JsonConverters.Messages;

public class MessageIdJsonConverter : JsonConverter<MessageId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(MessageId);
    }

    public override MessageId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString();

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