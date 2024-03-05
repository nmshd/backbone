using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.ValueConverters;

public class ExhaustionDateValueConverter : ValueConverter<ExhaustionDate, DateTime>
{
    public ExhaustionDateValueConverter() : this(null)
    {
    }

    public ExhaustionDateValueConverter(ConverterMappingHints? mappingHints)
        : base(
            v => v.Value,
            v => new ExhaustionDate(v),
            mappingHints
        )
    {
    }
}
