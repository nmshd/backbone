using Backbone.Modules.Announcements.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Database.ValueConverters;

public class AnnouncementLanguageEntityFrameworkValueConverter : ValueConverter<AnnouncementLanguage, string>
{
    public AnnouncementLanguageEntityFrameworkValueConverter() : this(new ConverterMappingHints())
    {
    }

    public AnnouncementLanguageEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id.Value,
            value => AnnouncementLanguage.Parse(value),
            mappingHints
        )
    {
    }
}
