using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Relationships.Domain.Ids;

namespace Relationships.Infrastructure.Persistence.Database.ValueConverters;

public class RelationshipChangeIdEntityFrameworkValueConverter : ValueConverter<RelationshipChangeId, string>
{
    public RelationshipChangeIdEntityFrameworkValueConverter() : this(null) { }

    public RelationshipChangeIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => RelationshipChangeId.Parse(value),
            mappingHints
        ) { }
}
