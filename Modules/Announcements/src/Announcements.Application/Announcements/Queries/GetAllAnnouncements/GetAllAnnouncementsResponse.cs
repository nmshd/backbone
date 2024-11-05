using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Announcements.Application.Announcements.DTOs;
using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application.Announcements.Queries.GetAllAnnouncements;

public class GetAllAnnouncementsResponse : CollectionResponseBase<AnnouncementDTO>
{
    public GetAllAnnouncementsResponse(IEnumerable<Announcement> items) : base(items.Select(a => new AnnouncementDTO(a)))
    {
    }
}
