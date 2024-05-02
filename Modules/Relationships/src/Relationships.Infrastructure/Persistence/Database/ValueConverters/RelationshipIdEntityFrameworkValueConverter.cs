using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.ValueConverters;

public class RelationshipIdEntityFrameworkValueConverter : ValueConverter<RelationshipId, string>
{
    public RelationshipIdEntityFrameworkValueConverter() : this(null)
    {
    }

    public RelationshipIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => RelationshipId.Parse(value),
            mappingHints
        )
    {
    }
}
