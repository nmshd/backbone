using System.Dynamic;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.ValueConverters;
public class ExternalEventPayloadEntityFrameworkValueConverter : ValueConverter<object, string>
{
    public ExternalEventPayloadEntityFrameworkValueConverter() : base(
        o => JsonConvert.SerializeObject(o),
        s => JsonConvert.DeserializeObject<ExpandoObject>(s)!)
    { }
}
