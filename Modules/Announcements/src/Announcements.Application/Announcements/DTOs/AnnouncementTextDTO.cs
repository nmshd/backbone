using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application.Announcements.DTOs;

public class AnnouncementTextDTO
{
    public AnnouncementTextDTO(AnnouncementText announcementText)
    {
        Language = announcementText.Language.Value;
        Title = announcementText.Title;
        Body = announcementText.Body;
    }

    public string Language { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}
