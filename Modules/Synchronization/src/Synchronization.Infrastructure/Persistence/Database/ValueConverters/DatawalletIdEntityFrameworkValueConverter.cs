using Backbone.Modules.Synchronization.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class DatawalletIdEntityFrameworkValueConverter : ValueConverter<DatawalletId, string>
{
    public DatawalletIdEntityFrameworkValueConverter() : this(new ConverterMappingHints(DatawalletId.MAX_LENGTH)) { }

    public DatawalletIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id.StringValue,
            value => DatawalletId.Parse(value),
            mappingHints?.With(new ConverterMappingHints(DatawalletId.MAX_LENGTH))
        )
    { }
}
