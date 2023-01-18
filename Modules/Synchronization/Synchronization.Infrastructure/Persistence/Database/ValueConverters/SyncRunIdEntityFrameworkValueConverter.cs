using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Synchronization.Domain.Entities.Sync;

namespace Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class SyncRunIdEntityFrameworkValueConverter : ValueConverter<SyncRunId, string>
{
    public SyncRunIdEntityFrameworkValueConverter() : this(new ConverterMappingHints(SyncRunId.MAX_LENGTH)) { }

    public SyncRunIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => SyncRunId.Parse(value),
            mappingHints?.With(new ConverterMappingHints(SyncRunId.MAX_LENGTH))
        ) { }
}
