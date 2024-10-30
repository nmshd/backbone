using Backbone.BuildingBlocks.Domain;

namespace Backbone.Modules.Announcements.Domain.Entities;

public class AnnouncementText : Entity
{
    public AnnouncementText()
    {
        AnnouncementId = null!;
        Title = null!;
        Body = null!;
    }

    public AnnouncementText(AnnouncementId announcementId, AnnouncementLanguage language, string title, byte[] body)
    {
        AnnouncementId = announcementId;
        Language = language;
        Title = title;
        Body = body;
    }

    public AnnouncementId AnnouncementId { get; }
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
