using Backbone.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;

public class UsernameValueConverter : ValueConverter<Username, string>
{
    public UsernameValueConverter() : this(null)
    {
    }

    public UsernameValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => Username.Parse(value),
            mappingHints
        )
    {
    }
}

public class NullableUsernameValueConverter : ValueConverter<Username?, string?>
{
    public NullableUsernameValueConverter() : this(null)
    {
    }

    public NullableUsernameValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id == null ? null : id.Value,
            value => value == null ? null : Username.Parse(value),
            mappingHints
        )
    {
    }
}
