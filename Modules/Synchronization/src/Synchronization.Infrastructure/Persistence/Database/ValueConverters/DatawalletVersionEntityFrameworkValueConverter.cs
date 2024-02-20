using Backbone.Modules.Synchronization.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class DatawalletVersionEntityFrameworkValueConverter : ValueConverter<Datawallet.DatawalletVersion, ushort>
{
    public DatawalletVersionEntityFrameworkValueConverter() : this(null) { }

    public DatawalletVersionEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => new Datawallet.DatawalletVersion(value),
            mappingHints
        )
    { }
}
