using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Announcements.Domain.Ids;

namespace Backbone.Modules.Announcements.Domain.Entities;
public class AnnouncementText : Entity
{
    public AnnouncementText()
    {
        Id = null!;
        Title = null!;
        Body = null!;
    }

    public AnnouncementText(AnnouncementLanguage language, string title, byte[] body)
    {
        Id = AnnouncementTextId.New();
        Language = language;
        Title = title;
        Body = body;
    }

    public AnnouncementTextId Id { get; set; }
    public AnnouncementLanguage Language { get; set; }
    public string Title { get; set; }
    public byte[] Body { get; set; }
}

public enum AnnouncementLanguage
{
    English,
    German,
    Portuguese
}
