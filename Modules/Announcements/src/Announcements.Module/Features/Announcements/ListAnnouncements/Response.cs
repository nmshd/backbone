using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Announcements.Module.Features.Announcements.Shared;
using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncements;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListAnnouncementsResponse")]
public class Response : CollectionResponseBase<AnnouncementDTO>
{
    public Response(IEnumerable<Announcement> items) : base(items.Select(a => new AnnouncementDTO(a)))
    {
    }
}
