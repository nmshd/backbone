using System.Dynamic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class ExternalEventPayloadEntityFrameworkValueConverter : ValueConverter<object, string>
{
    private static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS = new() { IncludeFields = true };

    public ExternalEventPayloadEntityFrameworkValueConverter() : base(
        o => JsonSerializer.Serialize(o, JSON_SERIALIZER_OPTIONS),
        s => JsonSerializer.Deserialize<ExpandoObject>(s, JSON_SERIALIZER_OPTIONS)!)
    {
    }
}
