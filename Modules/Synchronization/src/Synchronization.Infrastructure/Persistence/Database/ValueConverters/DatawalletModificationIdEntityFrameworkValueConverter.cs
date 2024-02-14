using Backbone.Modules.Synchronization.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class DatawalletModificationIdEntityFrameworkValueConverter : ValueConverter<DatawalletModificationId, string>
{
    public DatawalletModificationIdEntityFrameworkValueConverter() : this(null) { }

    public DatawalletModificationIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.StringValue,
            value => DatawalletModificationId.Parse(value),
            mappingHints?.With(new ConverterMappingHints(DatawalletModificationId.MAX_LENGTH))
        )
    { }
}
