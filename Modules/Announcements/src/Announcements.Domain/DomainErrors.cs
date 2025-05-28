using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Domain;

public class DomainErrors
{
    public static DomainError InvalidAnnouncementLanguage()
    {
        return new DomainError("error.platform.validation.invalidAnnouncementLanguage", "The Announcement Language must be a valid two letter ISO code.");
    }

    public static DomainError NonSilentAnnouncementCannotHaveIqlQuery()
    {
        return new DomainError("error.platform.validation.nonSilentAnnouncementCannotHaveIqlQuery", "An announcement with an IQL query must be silent.");
    }

    public static DomainError InvalidIqlQueryLengthForAnnouncement()
    {
        return new DomainError("error.platform.validation.invalidIqlQueryLengthForAnnouncement",
            $"The IQL query for an announcement must be between {AnnouncementIqlQuery.MIN_LENGTH} and {AnnouncementIqlQuery.MAX_LENGTH} characters long.");
    }
}
