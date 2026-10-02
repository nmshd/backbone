using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Modules.Announcements.Module.Features.Announcements.Shared;
using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.CreateAnnouncement;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateAnnouncementCommand")]
public class Command : IRequest<AnnouncementDTO>
{
    public required AnnouncementSeverity Severity { get; set; }
    public required bool IsSilent { get; set; }
    public required List<CreateAnnouncementCommandText> Texts { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public List<CreateAnnouncementCommandAction> Actions { get; set; } = [];

    public List<string> Recipients { get; set; } = [];
    public string? IqlQuery { get; set; }
}

public class CreateAnnouncementCommandAction
{
    public required Dictionary<string, string> DisplayName { get; set; }
    public required string Link { get; set; }
}

public class CreateAnnouncementCommandText
{
    public required string Language { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
}
