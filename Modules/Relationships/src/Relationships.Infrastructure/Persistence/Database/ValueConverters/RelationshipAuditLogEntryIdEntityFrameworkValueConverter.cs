using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.ValueConverters;

public class RelationshipAuditLogEntryIdEntityFrameworkValueConverter : ValueConverter<RelationshipAuditLogEntryId, string>
{
    public RelationshipAuditLogEntryIdEntityFrameworkValueConverter() : this(null)
    {
    }

    public RelationshipAuditLogEntryIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.StringValue,
            value => RelationshipAuditLogEntryId.Parse(value),
            mappingHints
        )
    {
    }
}
