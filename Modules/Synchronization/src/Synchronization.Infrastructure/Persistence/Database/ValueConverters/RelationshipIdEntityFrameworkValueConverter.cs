using Backbone.Modules.Synchronization.Domain.Entities.Relationships;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.ValueConverters;

public class RelationshipIdEntityFrameworkValueConverter : ValueConverter<RelationshipId, string>
{
    public RelationshipIdEntityFrameworkValueConverter() : this(new ConverterMappingHints(RelationshipId.MAX_LENGTH))
    {
    }

    public RelationshipIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id.Value,
            value => RelationshipId.Parse(value),
            mappingHints.With(new ConverterMappingHints(RelationshipId.MAX_LENGTH))
        )
    {
    }
}
