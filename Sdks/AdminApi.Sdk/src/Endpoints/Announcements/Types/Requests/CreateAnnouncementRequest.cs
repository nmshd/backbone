namespace Backbone.AdminApi.Sdk.Endpoints.Announcements.Types.Requests;

public class CreateAnnouncementRequest
{
    public required AnnouncementSeverity Severity { get; set; }
    public required bool IsSilent { get; set; }
    public required List<CreateAnnouncementRequestText> Texts { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public required List<CreateAnnouncementRequestAction> Actions { get; set; }
}

public class CreateAnnouncementRequestText
{
    public required string Language { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
}

public class CreateAnnouncementRequestAction
{
    public required Dictionary<string, string> DisplayName { get; set; }
    public required string Link { get; set; }
}
