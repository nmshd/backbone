using Backbone.Modules.Announcements.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Database.ValueConverters;

public class AnnouncementActionIdEntityFrameworkValueConverter : ValueConverter<AnnouncementActionId, string>
{
    public AnnouncementActionIdEntityFrameworkValueConverter() : this(new ConverterMappingHints())
    {
    }

    public AnnouncementActionIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id.Value,
            value => AnnouncementActionId.Parse(value),
            mappingHints
        )
    {
    }
}
