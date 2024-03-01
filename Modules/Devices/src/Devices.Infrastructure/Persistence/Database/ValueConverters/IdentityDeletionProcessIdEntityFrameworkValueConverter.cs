using Backbone.Modules.Devices.Domain.Entities.Identities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.ValueConverters;

public class IdentityDeletionProcessIdEntityFrameworkValueConverter : ValueConverter<IdentityDeletionProcessId, string>
{
    public IdentityDeletionProcessIdEntityFrameworkValueConverter() : this(null)
    {
    }

    public IdentityDeletionProcessIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => IdentityDeletionProcessId.Create(value).Value,
            mappingHints
        )
    {
    }
}

public class NullableIdentityDeletionProcessIdEntityFrameworkValueConverter : ValueConverter<IdentityDeletionProcessId?, string?>
{
    public NullableIdentityDeletionProcessIdEntityFrameworkValueConverter() : this(null)
    {
    }

    public NullableIdentityDeletionProcessIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id == null ? null : id.Value,
            value => value == null ? null : IdentityDeletionProcessId.Create(value).Value,
            mappingHints
        )
    {
    }
}
