using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Modules.Announcements.Module.Features.Announcements.Shared;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncementsForActiveIdentityInLanguage;

public class Response : CollectionResponseBase<SingleLanguageAnnouncementDTO>
{
    public Response(IEnumerable<Announcement> items, AnnouncementLanguage language) : base(items.Select(a => new SingleLanguageAnnouncementDTO(a, language)))
    {
    }
}
