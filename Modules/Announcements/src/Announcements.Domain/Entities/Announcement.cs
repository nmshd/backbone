using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Announcements.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;

namespace Backbone.Modules.Announcements.Domain.Entities;

public class Announcement : Entity
{
    // ReSharper disable once UnusedMember.Local
    private Announcement()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        Texts = null!;
        Recipients = null!;
    }

    public Announcement(AnnouncementSeverity severity, List<AnnouncementText> texts, DateTime? expiresAt, List<AnnouncementRecipient> recipients)
    {
        Id = AnnouncementId.New();
        CreatedAt = SystemTime.UtcNow;
        ExpiresAt = expiresAt;
        Severity = severity;
        Texts = texts;
        Recipients = recipients;

        RaiseDomainEvent(new AnnouncementCreatedDomainEvent(this));
    }

    public AnnouncementId Id { get; }
    public DateTime CreatedAt { get; }
    public DateTime? ExpiresAt { get; }
    public AnnouncementSeverity Severity { get; }

    public List<AnnouncementText> Texts { get; }

    public List<AnnouncementRecipient> Recipients { get; }
}

public enum AnnouncementSeverity
{
    Low,
    Medium,
    High
}
