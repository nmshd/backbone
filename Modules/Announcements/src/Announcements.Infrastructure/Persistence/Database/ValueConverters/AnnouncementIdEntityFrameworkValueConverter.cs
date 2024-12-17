using Backbone.Modules.Announcements.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Database.ValueConverters;

public class AnnouncementIdEntityFrameworkValueConverter : ValueConverter<AnnouncementId, string>
{
    public AnnouncementIdEntityFrameworkValueConverter() : this(new ConverterMappingHints())
    {
    }

    public AnnouncementIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id.Value,
            value => AnnouncementId.Parse(value),
            mappingHints
        )
    {
    }
}
