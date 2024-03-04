using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;

public class DateTimeValueConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeValueConverter() : this(null)
    {
    }

    public DateTimeValueConverter(ConverterMappingHints? mappingHints)
        : base(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
            mappingHints
        )
    {
    }
}

public class NullableDateTimeValueConverter : ValueConverter<DateTime?, DateTime?>
{
    public NullableDateTimeValueConverter() : this(null)
    {
    }

    public NullableDateTimeValueConverter(ConverterMappingHints? mappingHints)
        : base(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
            mappingHints
        )
    {
    }
}
