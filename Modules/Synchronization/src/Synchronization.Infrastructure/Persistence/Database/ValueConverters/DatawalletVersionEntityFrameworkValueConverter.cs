using Backbone.Synchronization.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class DatawalletVersionEntityFrameworkValueConverter : ValueConverter<Datawallet.DatawalletVersion, ushort>
{
    public DatawalletVersionEntityFrameworkValueConverter() : this(null) { }

    public DatawalletVersionEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id.Value,
            value => new Datawallet.DatawalletVersion(value),
            mappingHints
        )
    { }
}
