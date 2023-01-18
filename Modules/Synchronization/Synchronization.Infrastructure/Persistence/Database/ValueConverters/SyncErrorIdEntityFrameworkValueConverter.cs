using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Synchronization.Domain.Entities.Sync;

namespace Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class SyncErrorIdEntityFrameworkValueConverter : ValueConverter<SyncErrorId, string>
{
    public SyncErrorIdEntityFrameworkValueConverter() : this(new ConverterMappingHints(SyncErrorId.MAX_LENGTH)) { }

    public SyncErrorIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => SyncErrorId.Parse(value),
            mappingHints?.With(new ConverterMappingHints(SyncErrorId.MAX_LENGTH))
        ) { }
}
