using Backbone.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;

public class IdentityAddressValueConverter : ValueConverter<IdentityAddress, string>
{
    public IdentityAddressValueConverter() : this(null)
    {
    }

    public IdentityAddressValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.StringValue,
            value => IdentityAddress.ParseUnsafe(value.Trim()),
            mappingHints
        )
    {
    }
}
public class NullableIdentityAddressValueConverter : ValueConverter<IdentityAddress?, string?>
{
    public NullableIdentityAddressValueConverter() : this(null)
    {
    }

    public NullableIdentityAddressValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => value == null ? null : IdentityAddress.ParseUnsafe(value.Trim()),
            mappingHints
        )
    {
    }
}
