using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application.Announcements.DTOs;

/**
 * This class is used when a user's device asks for announcements. In that case we return
 * just the translation for the device's communication language.
 */
public class SingleLanguageAnnouncementDTO
{
    public SingleLanguageAnnouncementDTO(Announcement announcement, AnnouncementLanguage language)
    {
        Id = announcement.Id.Value;
        CreatedAt = announcement.CreatedAt;
        ExpiresAt = announcement.ExpiresAt;
        Severity = announcement.Severity;

        var textInLanguage = announcement.Texts.SingleOrDefault(t => t.Language == language) ??
                             announcement.Texts.Single(t => t.Language == AnnouncementLanguage.DEFAULT_LANGUAGE);

        Title = textInLanguage.Title;
        Body = textInLanguage.Body;
        Actions = announcement.Actions.Select(a => new SingleLanguageAnnouncementActionDTO(a, language));
    }

    public string Id { get; }
    public DateTime CreatedAt { get; }
    public DateTime? ExpiresAt { get; }
    public AnnouncementSeverity Severity { get; }
    public string Title { get; set; }
    public string Body { get; set; }
    public IEnumerable<SingleLanguageAnnouncementActionDTO> Actions { get; set; }
}

public class SingleLanguageAnnouncementActionDTO
{
    public SingleLanguageAnnouncementActionDTO(AnnouncementAction announcementAction, AnnouncementLanguage language)
    {
        DisplayName = announcementAction.DisplayName.TryGetValue(language, out var displayName) ? displayName : announcementAction.DisplayName[AnnouncementLanguage.DEFAULT_LANGUAGE];
        Link = announcementAction.Link;
    }

    public string DisplayName { get; }
    public string Link { get; }
}
