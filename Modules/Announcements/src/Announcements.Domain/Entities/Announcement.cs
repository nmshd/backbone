using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Announcements.Domain.Ids;

namespace Backbone.Modules.Announcements.Domain.Entities
{
    public class Announcement : Entity
    {
        private Announcement()
        {
            // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
            Id = null!;
        }

        public Announcement(AnnouncementSeverity severity, DateTime? expiresAt = null)
        {
            Id = AnnouncementId.New();
            CreatedAt = DateTime.Now;
            ExpiresAt = expiresAt;
            Severity = severity;
        }

        public AnnouncementId Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public AnnouncementSeverity Severity { get; set; }
    }
}

public enum AnnouncementSeverity
{
    Low,
    Medium,
    High
}
