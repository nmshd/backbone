using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Relationships.Domain.Ids;

namespace Relationships.Infrastructure.Persistence.Database.ValueConverters;

public class RelationshipTemplateIdEntityFrameworkValueConverter : ValueConverter<RelationshipTemplateId, string>
{
    public RelationshipTemplateIdEntityFrameworkValueConverter() : this(null) { }

    public RelationshipTemplateIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => RelationshipTemplateId.Parse(value),
            mappingHints
        ) { }
}
