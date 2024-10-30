using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application.Announcements.DTOs;

public class AnnouncementTextDTO
{
    public AnnouncementTextDTO(AnnouncementText announcementText)
    {
        Language = announcementText.Language;
        Title = announcementText.Title;
        Body = announcementText.Body;
    }

    public AnnouncementLanguage Language { get; set; }
    public string Title { get; set; }
    public byte[] Body { get; set; }
}
