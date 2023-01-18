using Messages.Domain.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Messages.Infrastructure.Persistence.Database.ValueConverters;

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
