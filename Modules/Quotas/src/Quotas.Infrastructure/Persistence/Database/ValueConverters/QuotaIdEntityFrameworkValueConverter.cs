using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.ValueConverters;

public class QuotaIdEntityFrameworkValueConverter : ValueConverter<QuotaId, string>
{
    public QuotaIdEntityFrameworkValueConverter() : this(null)
    {
    }

    public QuotaIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => QuotaId.Create(value).Value,
            mappingHints
        )
    {
    }
}

public class NullableQuotaIdValueConverter : ValueConverter<QuotaId?, string?>
{
    public NullableQuotaIdValueConverter() : this(null)
    {
    }

    public NullableQuotaIdValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id == null ? null : id.Value,
            value => value == null ? null : QuotaId.Create(value).Value,
            mappingHints
        )
    {
    }
}
