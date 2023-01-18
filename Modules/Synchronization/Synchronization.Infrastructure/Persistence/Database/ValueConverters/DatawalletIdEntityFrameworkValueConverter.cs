using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Synchronization.Domain.Entities;

namespace Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class DatawalletIdEntityFrameworkValueConverter : ValueConverter<DatawalletId, string>
{
    public DatawalletIdEntityFrameworkValueConverter() : this(new ConverterMappingHints(DatawalletId.MAX_LENGTH)) { }

    public DatawalletIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => DatawalletId.Parse(value),
            mappingHints?.With(new ConverterMappingHints(DatawalletId.MAX_LENGTH))
        ) { }
}
