using Backbone.Modules.Announcements.Application.Announcements.DTOs;
using Backbone.Modules.Announcements.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.CreateAnnouncement;

public class CreateAnnouncementCommand : IRequest<AnnouncementDTO>
{
    public required AnnouncementSeverity Severity { get; set; }
    public required List<CreateAnnouncementCommandText> Texts { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public List<string> Recipients { get; set; }
}

public class CreateAnnouncementCommandText
{
    public required string Language { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
}
