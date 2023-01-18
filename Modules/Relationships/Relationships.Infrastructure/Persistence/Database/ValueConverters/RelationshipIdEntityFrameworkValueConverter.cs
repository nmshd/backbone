using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Relationships.Domain.Ids;

namespace Relationships.Infrastructure.Persistence.Database.ValueConverters;

public class RelationshipIdEntityFrameworkValueConverter : ValueConverter<RelationshipId, string>
{
    public RelationshipIdEntityFrameworkValueConverter() : this(null) { }

    public RelationshipIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => RelationshipId.Parse(value),
            mappingHints
        ) { }
}
