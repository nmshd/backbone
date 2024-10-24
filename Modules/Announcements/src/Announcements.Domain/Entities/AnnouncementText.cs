using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Announcements.Domain.Ids;

namespace Backbone.Modules.Announcements.Domain.Entities;
public class AnnouncementText : Entity
{
    public AnnouncementTextId AnnouncementId { get; set; }
    public AnnouncementLanguage Language { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}

public enum AnnouncementLanguage
{
    English,
    German,
    Portuguese
}
