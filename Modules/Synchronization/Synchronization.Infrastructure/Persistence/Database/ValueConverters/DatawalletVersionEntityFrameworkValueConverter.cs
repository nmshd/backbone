using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Synchronization.Domain.Entities;

namespace Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class DatawalletVersionEntityFrameworkValueConverter : ValueConverter<Datawallet.DatawalletVersion, ushort>
{
    public DatawalletVersionEntityFrameworkValueConverter() : this(null) { }

    public DatawalletVersionEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id.Value,
            value => new Datawallet.DatawalletVersion(value),
            mappingHints
        ) { }
}
