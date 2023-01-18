using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Synchronization.Domain.Entities.Sync;

namespace Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class ExternalEventIdEntityFrameworkValueConverter : ValueConverter<ExternalEventId, string>
{
    public ExternalEventIdEntityFrameworkValueConverter() : this(new ConverterMappingHints(ExternalEventId.MAX_LENGTH)) { }

    public ExternalEventIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => ExternalEventId.Parse(value),
            mappingHints?.With(new ConverterMappingHints(ExternalEventId.MAX_LENGTH))
        ) { }
}
