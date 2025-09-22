using Backbone.Modules.Devices.Domain.Entities.Identities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.ValueConverters;

public class FeatureFlagNameEntityFrameworkValueConverter : ValueConverter<FeatureFlagName, string>
{
    public FeatureFlagNameEntityFrameworkValueConverter() : this(null)
    {
    }

    public FeatureFlagNameEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => FeatureFlagName.Parse(value),
            mappingHints
        )
    {
    }
}
