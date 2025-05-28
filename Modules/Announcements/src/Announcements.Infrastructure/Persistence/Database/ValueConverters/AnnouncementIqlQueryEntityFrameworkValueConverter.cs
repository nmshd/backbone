using Backbone.Modules.Announcements.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Database.ValueConverters;

public class AnnouncementIqlQueryEntityFrameworkValueConverter : ValueConverter<AnnouncementIqlQuery?, string?>
{
    public AnnouncementIqlQueryEntityFrameworkValueConverter() : this(new ConverterMappingHints())
    {
    }

    public AnnouncementIqlQueryEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            valueObject => valueObject == null ? null : valueObject.Value,
            value => value == null ? null : AnnouncementIqlQuery.Parse(value),
            mappingHints
        )
    {
    }
}
