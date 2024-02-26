using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class ExternalEventIdEntityFrameworkValueConverter : ValueConverter<ExternalEventId, string>
{
    public ExternalEventIdEntityFrameworkValueConverter() : this(new ConverterMappingHints(ExternalEventId.MAX_LENGTH)) { }

    public ExternalEventIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id.StringValue,
            value => ExternalEventId.Parse(value),
            mappingHints.With(new ConverterMappingHints(ExternalEventId.MAX_LENGTH))
        )
    { }
}
