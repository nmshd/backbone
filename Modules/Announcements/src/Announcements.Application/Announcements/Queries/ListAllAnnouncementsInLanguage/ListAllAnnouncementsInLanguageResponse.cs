using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Announcements.Application.Announcements.DTOs;
using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.ListAllAnnouncementsInLanguage;

public class ListAllAnnouncementsInLanguageResponse : CollectionResponseBase<SingleLanguageAnnouncementDTO>
{
    public ListAllAnnouncementsInLanguageResponse(IEnumerable<Announcement> items, AnnouncementLanguage language) : base(items.Select(a => new SingleLanguageAnnouncementDTO(a, language)))
    {
    }
}
