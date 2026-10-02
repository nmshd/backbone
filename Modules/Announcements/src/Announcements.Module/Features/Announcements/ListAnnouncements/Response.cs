using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Modules.Announcements.Module.Features.Announcements.Shared;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncements;

public class Response : CollectionResponseBase<AnnouncementDTO>
{
    public Response(IEnumerable<Announcement> items) : base(items.Select(a => new AnnouncementDTO(a)))
    {
    }
}
