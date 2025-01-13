using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Domain.DomainEvents.Outgoing;

public class AnnouncementCreatedDomainEvent : DomainEvent
{
    public AnnouncementCreatedDomainEvent(Announcement announcement) : base($"{announcement.Id}/Created")
    {
        Id = announcement.Id.Value;
        Severity = announcement.Severity.ToString();
        Texts = announcement.Texts.Select(t => new AnnouncementCreatedDomainEventText(t)).ToList();
        Recipients = announcement.Recipients.Select(r => r.Address).ToList();
    }

    public string Id { get; }
    public string Severity { get; }
    public List<AnnouncementCreatedDomainEventText> Texts { get; }
    public List<string> Recipients { get; }
}

public class AnnouncementCreatedDomainEventText
{
    public AnnouncementCreatedDomainEventText(AnnouncementText announcementText)
    {
        Language = announcementText.Language.Value;
        Title = announcementText.Title;
        Body = announcementText.Body;
    }

    public string Language { get; }
    public string Title { get; }
    public string Body { get; }
}
