using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backbone.Modules.Announcements.Domain.Entities
{
    public class Announcement
    {
        public int Id { get; set; }
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
