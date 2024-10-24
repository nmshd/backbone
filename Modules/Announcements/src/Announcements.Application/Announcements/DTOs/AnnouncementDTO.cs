using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application.Announcements.DTOs
{
    public class AnnouncementDTO
    {
        public AnnouncementDTO(Announcement announcement)
        {
            Id = announcement.Id;
            CreatedAt = announcement.CreatedAt;
            ExpiresAt = announcement.ExpiresAt;
            Severity = announcement.Severity;
        }

        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public AnnouncementSeverity Severity { get; set; }
    }
}
