using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class SyncErrorIdEntityFrameworkValueConverter : ValueConverter<SyncErrorId, string>
{
    public SyncErrorIdEntityFrameworkValueConverter() : this(new ConverterMappingHints(SyncErrorId.MAX_LENGTH)) { }

    public SyncErrorIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id.StringValue,
            value => SyncErrorId.Parse(value),
            mappingHints.With(new ConverterMappingHints(SyncErrorId.MAX_LENGTH))
        )
    { }
}
