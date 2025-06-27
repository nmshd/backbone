using System.Dynamic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class ExternalEventPayloadEntityFrameworkValueConverter : ValueConverter<object, string>
{
    public ExternalEventPayloadEntityFrameworkValueConverter() : base(
        o => JsonSerializer.Serialize(o, new JsonSerializerOptions { IncludeFields = true }),
        s => JsonSerializer.Deserialize<ExpandoObject>(s, new JsonSerializerOptions { IncludeFields = true })!)
    {
    }
}
