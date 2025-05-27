using System.Text.Json.Serialization;

namespace Backbone.AdminApi.Sdk.Endpoints.Announcements.Types;

public class Announcement
{
    public required string Id { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? ExpiresAt { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required AnnouncementSeverity Severity { get; set; }

    public required IEnumerable<AnnouncementText> Texts { get; set; }

    public required IEnumerable<string> Recipients { get; set; }
    public required IEnumerable<AnnouncementAction> Actions { get; set; }
}

public class AnnouncementAction
{
    public required Dictionary<string, string> DisplayName { get; set; }
    public required string Link { get; set; }
}

public class AnnouncementText
{
    public required string Language { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
}

public enum AnnouncementSeverity
{
    Low,
    Medium,
    High
}
