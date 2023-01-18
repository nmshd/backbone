using System.Text.Json;
using System.Text.Json.Serialization;
using Enmeshed.StronglyTypedIds;
using Messages.Domain.Ids;

namespace Backbone.API.Mvc.JsonConverters.Messages;

public class FileIdJsonConverter : JsonConverter<FileId>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(FileId);
    }

    public override FileId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString();

        try
        {
            return FileId.Parse(id);
        }
        catch (InvalidIdException ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, FileId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.StringValue);
    }
}