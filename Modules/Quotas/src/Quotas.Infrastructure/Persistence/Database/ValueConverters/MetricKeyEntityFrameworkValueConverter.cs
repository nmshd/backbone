using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.ValueConverters;

public class MetricKeyEntityFrameworkValueConverter : ValueConverter<MetricKey, string>
{
    public MetricKeyEntityFrameworkValueConverter() : this(null)
    {
    }

    public MetricKeyEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            metricKey => metricKey.ToString(),
            value => (MetricKey)Enum.Parse(typeof(MetricKey), value),
            mappingHints
        )
    {
    }
}

public class NullableMetricKeyValueConverter : ValueConverter<MetricKey?, string?>
{
    public NullableMetricKeyValueConverter() : this(null)
    {
    }

    public NullableMetricKeyValueConverter(ConverterMappingHints? mappingHints)
        : base(
            metricKey => metricKey == null ? null : metricKey.ToString(),
            value => value == null ? null : (MetricKey)Enum.Parse(typeof(MetricKey), value),
            mappingHints
        )
    {
    }
}
