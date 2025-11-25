using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.ValueConverters;

public class TierNameEntityFrameworkValueConverter : ValueConverter<TierName, string>
{
    public TierNameEntityFrameworkValueConverter() : this(null)
    {
    }

    public TierNameEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => TierName.Create(value).Value,
            mappingHints
        )
    {
    }
}

public class NullableTierNameValueConverter : ValueConverter<TierName?, string?>
{
    public NullableTierNameValueConverter() : this(null)
    {
    }

    public NullableTierNameValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id == null ? null : id.Value,
            value => value == null ? null : TierName.Create(value).Value,
            mappingHints
        )
    {
    }
}
