using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application.Announcements.DTOs;

public class SingleLanguageAnnouncementDTO
{
    public SingleLanguageAnnouncementDTO(Announcement announcement, AnnouncementLanguage language)
    {
        Id = announcement.Id.Value;
        CreatedAt = announcement.CreatedAt;
        ExpiresAt = announcement.ExpiresAt;
        Severity = announcement.Severity;

        var textInLanguage = announcement.Texts.SingleOrDefault(t => t.Language == language) ??
                             announcement.Texts.Single(t => t.Language == AnnouncementLanguage.ParseUnsafe("en"));

        Text = new AnnouncementTextDTO(textInLanguage);
    }

    public string Id { get; }
    public DateTime CreatedAt { get; }
    public DateTime? ExpiresAt { get; }
    public AnnouncementSeverity Severity { get; }
    public AnnouncementTextDTO Text { get; }
}
