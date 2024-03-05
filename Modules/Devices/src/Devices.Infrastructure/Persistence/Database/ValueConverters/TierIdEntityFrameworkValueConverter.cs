using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.ValueConverters;

public class TierIdEntityFrameworkValueConverter : ValueConverter<TierId, string>
{
    public TierIdEntityFrameworkValueConverter() : this(null)
    {
    }

    public TierIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => TierId.Create(value).Value,
            mappingHints
        )
    {
    }
}

public class NullableTierIdValueConverter : ValueConverter<TierId?, string?>
{
    public NullableTierIdValueConverter() : this(null)
    {
    }

    public NullableTierIdValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id == null ? null : id.Value,
            value => value == null ? null : TierId.Create(value).Value,
            mappingHints
        )
    {
    }
}
