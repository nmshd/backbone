using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Announcements.Module.Features.Announcements.Shared;
using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncementsInLanguage;

public class ListAnnouncementsInLanguageResponse : CollectionResponseBase<SingleLanguageAnnouncementDTO>
{
    public ListAnnouncementsInLanguageResponse(IEnumerable<Announcement> items, AnnouncementLanguage language) : base(items.Select(a => new SingleLanguageAnnouncementDTO(a, language)))
    {
    }
}
