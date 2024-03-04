using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.ValueConverters;

public class TierQuotaDefinitionIdEntityFrameworkValueConverter : ValueConverter<TierQuotaDefinitionId, string>
{
    public TierQuotaDefinitionIdEntityFrameworkValueConverter() : this(null)
    {
    }

    public TierQuotaDefinitionIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => TierQuotaDefinitionId.Create(value).Value,
            mappingHints
        )
    {
    }
}

public class NullableTierQuotaDefinitionIdValueConverter : ValueConverter<TierQuotaDefinitionId?, string?>
{
    public NullableTierQuotaDefinitionIdValueConverter() : this(null)
    {
    }

    public NullableTierQuotaDefinitionIdValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id == null ? null : id.Value,
            value => value == null ? null : TierQuotaDefinitionId.Create(value).Value,
            mappingHints
        )
    {
    }
}
