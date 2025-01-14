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
            Texts = announcement.Texts.Select(t => new AnnouncementTextDTO(t));
            Recipients = announcement.Recipients.Select(r => new AnnouncementRecipientDTO(r));
        }

        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public AnnouncementSeverity Severity { get; set; }
        public IEnumerable<AnnouncementTextDTO> Texts { get; set; }
        public IEnumerable<AnnouncementRecipientDTO> Recipients { get; set; }
    }

    public record AnnouncementRecipientDTO
    {
        public AnnouncementRecipientDTO(AnnouncementRecipient announcementRecipient)
        {
            DeviceId = announcementRecipient.DeviceId;
            Address = announcementRecipient.Address;
        }

        public string DeviceId { get; set; }
        public string Address { get; set; }
    }
}
