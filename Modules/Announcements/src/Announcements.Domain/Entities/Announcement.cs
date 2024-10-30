using Backbone.BuildingBlocks.Domain;

namespace Backbone.Modules.Announcements.Domain.Entities
{
    public class Announcement : Entity
    {
        // ReSharper disable once UnusedMember.Local
        private Announcement(List<AnnouncementText> texts)
        {
            // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
            Id = null!;
            Texts = texts;
        }

        public Announcement(AnnouncementSeverity severity, List<AnnouncementText> texts, DateTime? expiresAt)
        {
            Id = AnnouncementId.New();
            CreatedAt = DateTime.Now;
            ExpiresAt = expiresAt;
            Severity = severity;
            Texts = texts;
        }

        public AnnouncementId Id { get; }
        public DateTime CreatedAt { get; }
        public DateTime? ExpiresAt { get; }
        public AnnouncementSeverity Severity { get; }

        public List<AnnouncementText> Texts { get; }
    }
}

public enum AnnouncementSeverity
{
    Low,
    Medium,
    High
}
