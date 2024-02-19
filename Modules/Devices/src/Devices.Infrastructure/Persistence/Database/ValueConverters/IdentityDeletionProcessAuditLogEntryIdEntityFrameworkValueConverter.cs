using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.ValueConverters;

public class IdentityDeletionProcessAuditLogEntryIdEntityFrameworkValueConverter : ValueConverter<IdentityDeletionProcessAuditLogEntryId, string>
{
    public IdentityDeletionProcessAuditLogEntryIdEntityFrameworkValueConverter() : this(null)
    {
    }

    public IdentityDeletionProcessAuditLogEntryIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => IdentityDeletionProcessAuditLogEntryId.Create(value).Value,
            mappingHints
        )
    {
    }
}

public class NullableIdentityDeletionProcessAuditLogEntryIdEntityFrameworkValueConverter : ValueConverter<IdentityDeletionProcessAuditLogEntryId?, string?>
{
    public NullableIdentityDeletionProcessAuditLogEntryIdEntityFrameworkValueConverter() : this(null)
    {
    }

    public NullableIdentityDeletionProcessAuditLogEntryIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id == null ? null : id.Value,
            value => value == null ? null : IdentityDeletionProcessAuditLogEntryId.Create(value).Value,
            mappingHints
        )
    {
    }
}
