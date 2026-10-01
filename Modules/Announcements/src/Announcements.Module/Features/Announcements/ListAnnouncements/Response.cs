using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Announcements.Module.Features.Announcements.Shared;
using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncements;

public class ListAnnouncementsResponse : CollectionResponseBase<AnnouncementDTO>
{
    public ListAnnouncementsResponse(IEnumerable<Announcement> items) : base(items.Select(a => new AnnouncementDTO(a)))
    {
    }
}
