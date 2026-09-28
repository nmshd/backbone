using Backbone.Modules.Announcements.Contracts.DomainEvents;
using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Domain.DomainEvents;

public static class AnnouncementCreatedDomainEventExtensions
{
    extension(AnnouncementCreatedDomainEvent)
    {
        public static AnnouncementCreatedDomainEvent Create(Announcement announcement)
        {
            return new AnnouncementCreatedDomainEvent
            {
                DomainEventId = $"{announcement.Id}/Created",
                Id = announcement.Id.Value,
                Severity = announcement.Severity.ToString(),
                IsSilent = announcement.IsSilent,
                Texts = announcement.Texts.Select(t => new AnnouncementCreatedDomainEventText
                {
                    Language = t.Language.Value,
                    Title = t.Title,
                    Body = t.Body
                }).ToList(),
                Recipients = announcement.Recipients.Select(r => r.Address.Value).ToList()
            };
        }
    }
}
