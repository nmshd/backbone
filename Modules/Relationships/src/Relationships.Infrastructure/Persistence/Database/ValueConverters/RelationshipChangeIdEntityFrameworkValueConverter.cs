using Backbone.Modules.Relationships.Domain.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.ValueConverters;

public class RelationshipChangeIdEntityFrameworkValueConverter : ValueConverter<RelationshipChangeId, string>
{
    public RelationshipChangeIdEntityFrameworkValueConverter() : this(null)
    {
    }

    public RelationshipChangeIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => RelationshipChangeId.Parse(value),
            mappingHints
        )
    {
    }
}
