using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Announcements.Application.Announcements.DTOs;
using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.GetAllAnnouncementsInLanguage;

public class GetAllAnnouncementsInLanguageResponse : CollectionResponseBase<SingleLanguageAnnouncementDTO>
{
    public GetAllAnnouncementsInLanguageResponse(IEnumerable<Announcement> items, AnnouncementLanguage language) : base(items.Select(a => new SingleLanguageAnnouncementDTO(a, language)))
    {
    }
}
