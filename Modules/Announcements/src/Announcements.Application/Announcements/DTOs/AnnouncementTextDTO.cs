using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application.Announcements.DTOs;
public class AnnouncementTextDTO
{
    public AnnouncementTextDTO(AnnouncementText announcementText)
    {
        AnnouncementId = announcementText.AnnouncementTextId;
        Language = announcementText.Language;
        Title = announcementText.Title;
        Body = announcementText.Body;
    }

    public string AnnouncementId { get; set; }
    public AnnouncementLanguage Language { get; set; }
    public string Title { get; set; }
    public byte[] Body { get; set; }
}
