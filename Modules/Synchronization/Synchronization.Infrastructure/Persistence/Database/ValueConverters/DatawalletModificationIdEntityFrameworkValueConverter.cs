using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Synchronization.Domain.Entities;

namespace Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class DatawalletModificationIdEntityFrameworkValueConverter : ValueConverter<DatawalletModificationId, string>
{
    public DatawalletModificationIdEntityFrameworkValueConverter() : this(null) { }

    public DatawalletModificationIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => DatawalletModificationId.Parse(value),
            mappingHints?.With(new ConverterMappingHints(DatawalletModificationId.MAX_LENGTH))
        ) { }
}
