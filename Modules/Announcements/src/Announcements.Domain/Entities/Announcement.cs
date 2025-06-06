using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
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
        IqlQuery = null!;
    }

    public Announcement(AnnouncementSeverity severity, bool isSilent, List<AnnouncementText> texts, DateTime? expiresAt, IEnumerable<AnnouncementRecipient> recipients, AnnouncementIqlQuery? iqlQuery)
    {
        if (!isSilent && iqlQuery != null)
            throw new DomainException(DomainErrors.NonSilentAnnouncementCannotHaveIqlQuery());

        Id = AnnouncementId.New();
        CreatedAt = SystemTime.UtcNow;
        ExpiresAt = expiresAt;
        IqlQuery = iqlQuery;
        Severity = severity;
        Texts = texts;
        Recipients = [.. recipients];
        IsSilent = isSilent;

        RaiseDomainEvent(new AnnouncementCreatedDomainEvent(this));
    }

    public AnnouncementId Id { get; }
    public DateTime CreatedAt { get; }
    public DateTime? ExpiresAt { get; }
    public AnnouncementIqlQuery? IqlQuery { get; }
    public AnnouncementSeverity Severity { get; }
    public bool IsSilent { get; }

    public List<AnnouncementText> Texts { get; }

    public List<AnnouncementRecipient> Recipients { get; }

    public static Expression<Func<Announcement, bool>> IsForRecipient(IdentityAddress recipientAddress) => a => a.Recipients.Count == 0 || a.Recipients.Any(r => r.Address == recipientAddress);
}

public enum AnnouncementSeverity
{
    Low,
    Medium,
    High
}
