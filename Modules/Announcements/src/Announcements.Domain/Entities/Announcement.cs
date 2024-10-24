using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Announcements.Domain.Ids;

namespace Backbone.Modules.Announcements.Domain.Entities
{
    public class Announcement : Entity
    {
        public AnnouncementId Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public AnnouncementSeverity Severity { get; set; }
    }
}

public enum AnnouncementSeverity
{
    Low,
    Medium,
    High
}
