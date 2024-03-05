using System.Dynamic;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.ValueConverters;
public class ExternalEventIdPayloadEntityFrameworkValueConverter : ValueConverter<object, string>
{
    public ExternalEventIdPayloadEntityFrameworkValueConverter() : base(
        o => JsonConvert.SerializeObject(o),
        s => JsonConvert.DeserializeObject<ExpandoObject>(s)!)
    { }
}
