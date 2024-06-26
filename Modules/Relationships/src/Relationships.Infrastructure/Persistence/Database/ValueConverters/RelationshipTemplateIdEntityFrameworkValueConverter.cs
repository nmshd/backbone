using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.ValueConverters;

public class RelationshipTemplateIdEntityFrameworkValueConverter : ValueConverter<RelationshipTemplateId, string>
{
    public RelationshipTemplateIdEntityFrameworkValueConverter() : this(null)
    {
    }

    public RelationshipTemplateIdEntityFrameworkValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.Value,
            value => RelationshipTemplateId.Parse(value),
            mappingHints
        )
    {
    }
}
