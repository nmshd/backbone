﻿using Backbone.Modules.Announcements.Domain.Entities;

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
            Recipients = announcement.Recipients.Select(r => r.Address.ToString());
            IqlQuery = announcement.IqlQuery?.ToString();
        }


        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public AnnouncementSeverity Severity { get; set; }
        public IEnumerable<AnnouncementTextDTO> Texts { get; set; }
        public IEnumerable<string> Recipients { get; set; }
        public string? IqlQuery { get; set; }
        public bool IsSilent { get; set; }
    }
}
