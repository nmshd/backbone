namespace Backbone.AdminApi.Sdk.Endpoints.Announcements.Types.Requests;

public class CreateAnnouncementRequest
{
    public required AnnouncementSeverity Severity { get; set; }
    public required bool IsSilent { get; set; }
    public required List<CreateAnnouncementRequestText> Texts { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public string? IqlQuery { get; set; }
}

public class CreateAnnouncementRequestText
{
    public required string Language { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
}
