using Backbone.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Announcements.Domain;

public class DomainErrors
{
    public static DomainError InvalidAnnouncementLanguage()
    {
        return new DomainError("error.platform.validation.invalidAnnouncementLanguage", "The Announcement Language must be a valid two letter ISO code.");
    }
}
