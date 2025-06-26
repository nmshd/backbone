using System.Text.Json;
using System.Text.Json.Serialization;
using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Json;

public class PolymorphicEventConverter : JsonConverter<DomainEvent>
{
    public override DomainEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var root = jsonDoc.RootElement;

        if (!root.TryGetProperty("Type", out var typeProp))
            throw new JsonException("Missing 'Type' discriminator");

        var typeDiscriminator = typeProp.GetString()?.ToLower();

        if (typeDiscriminator == null || !DomainEventTypeRegistry.DISCRIMINATOR_TO_TYPE.TryGetValue(typeDiscriminator, out var targetType))
            throw new JsonException($"Unknown domain event type: {typeDiscriminator}");

        return (DomainEvent?)JsonSerializer.Deserialize(root.GetRawText(), targetType, options);
    }

    public override void Write(Utf8JsonWriter writer, DomainEvent value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}
