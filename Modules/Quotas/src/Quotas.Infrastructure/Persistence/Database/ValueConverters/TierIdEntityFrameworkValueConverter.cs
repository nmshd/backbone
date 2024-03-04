using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.ValueConverters;

public class TierIdEntityFrameworkValueConverter : ValueConverter<TierId, string>
{
    public TierIdEntityFrameworkValueConverter() : this(null)
    {
    }

    public TierIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => new TierId(value),
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
            value => value == null ? null : new TierId(value),
            mappingHints
        )
    {
    }
}
