using Backbone.Modules.Announcements.Domain.Ids;

namespace Backbone.Modules.Announcements.Domain.Entities;
public class AnnouncementText
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
